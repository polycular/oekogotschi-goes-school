using System;
using System.Collections.Generic;
using System.Timers;
using Flurl;
using JSONApi.ContractResolvers;
using JSONApi.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polycular.Persistance;
using Polycular.ResourceProvider;
using RSG;
using UnityEngine;
using Polycular;

namespace Http
{
	public class OAuth : IDisposable
    {
        public event Action OnRefreshTokenInvalid;

        readonly int RefreshExpiration;
        readonly int AccessExpirationDivisor;

        HttpResourceProvider httpProvider;
        Timer timer;


        public enum TokenType
        {
            ACCESS,
            REFRESH
        }

        public enum GrantType
        {
            USERNAME_PW,
            REFRESH
        }

        public OAuth(HttpResourceProvider provider, float timerIntervalInSec, int refreshExp, int accessExpDevisor)
        {
            httpProvider = provider;
            timer = new Timer(timerIntervalInSec * 1000);

            RefreshExpiration = refreshExp;
            AccessExpirationDivisor = accessExpDevisor;
        }

        public void StartService()
        {
            timer.Elapsed -= TimerElapsed;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        public void StopService()
        {
            timer.Elapsed -= TimerElapsed;
            timer.Stop();
        }

        public Promise<string> GetAccessToken()
        {
            var promise = new Promise<string>();

            if (!IsTimeExpired(TokenType.ACCESS))
            {
                promise.Resolve(httpProvider.User.AccessToken);
            }
            else
            {
                RefreshAccessToken()
					.Then(() => promise.Resolve(httpProvider.User.AccessToken))
					.Catch(ex => promise.Reject(ex));
            }

            return promise;
        }

        void TimerElapsed(object sender, EventArgs e)
        {
            if (IsTimeExpired(TokenType.ACCESS))
                RefreshAccessToken()
                    .Catch(ex => Debug.LogWarning(ex));
        }

        bool IsTimeExpired(TokenType type)
        {
            if (type == TokenType.ACCESS)
                return httpProvider.User.AccessExpiry.Subtract(DateTime.Now).TotalSeconds <= 0;
            else
                return httpProvider.User.RefreshExpiry.Subtract(DateTime.Now).TotalSeconds <= 0;
        }

        public Promise Register(string user, string secret)
        {
            Promise promise = new Promise();

            // User creation requires a specific request body which is created here
            Resource resource = new Resource();
            resource.Type = "user";
            resource.Attributes = new Dictionary<string, JToken>();
            resource.Attributes.Add("email", user);
            resource.Attributes.Add("password", secret);

            JSONApi.JSON.Object jsonapiObject = new JSONApi.JSON.Object();
            jsonapiObject.Data = resource;

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                ContractResolver = new ToCamelCaseContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(jsonapiObject, Formatting.None, jsonSettings);

            Url url = new Url(httpProvider.ServerConfig.UrlBase);
            url.AppendPathSegment(httpProvider.ServerConfig.UserEndpoint);

            httpProvider.Post(url, json, ContentType.JSON, false, false)
				.Then(response =>
				{
					promise.Resolve();
				})
				.Catch(ex =>
                {
                    Debug.LogWarning(ex);
                    promise.Reject(ex);
                });

            return promise;
        }

        public Promise Auth(string username, string password)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };

            return SendAuthRequest(body);
        }

        Promise Auth(string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            return SendAuthRequest(body);
        }

        Promise SendAuthRequest(Dictionary<string, string> body)
        {
            var promise = new Promise();

            // build request
            Url url = new Url(httpProvider.ServerConfig.UrlBase);
            url.AppendPathSegment(httpProvider.ServerConfig.OAuthEndpoint);

            httpProvider.Post(url, body, ContentType.JSON, false, false)
            .Then(response =>
            {
                UpdateOAuthInfo(response);

                Debug.Log("User authorized: " + httpProvider.User.ToString());
                promise.Resolve();
            })
            .Catch(ex =>
            {
                if (!(ex is TimeoutException)
                    && body["grant_type"].Equals("refresh_token")
                    && OnRefreshTokenInvalid != null)
                {
                    OnRefreshTokenInvalid();

					Eventbus.Instance.FireEvent<AuthFailureEvent>(new AuthFailureEvent());
                }

                Debug.LogError(ex);
                promise.Reject(ex);
            });

            return promise;
		}

        public Promise AuthFacebook(string userId, string accessToken)
        {
            var promise = new Promise();

            Dictionary<string, JToken> jsonDict = new Dictionary<string, JToken>()
                {
                    {"user_id", userId},
                    {"access_token", accessToken }
                };

            string json = JsonConvert.SerializeObject(jsonDict, Formatting.None);

            Url url = new Url(httpProvider.ServerConfig.UrlBase);
            url.AppendPathSegment(httpProvider.ServerConfig.FbEndpoint);

            httpProvider.Post(url, json, ContentType.JSON, false, false)
                .Then(response =>
                {
					UpdateOAuthInfo(response);
					promise.Resolve();
                })
                .Catch(exception =>
                {
                    Debug.LogError(exception);
                    promise.Reject(exception);
                });

            return promise;
        }

        Promise RefreshAccessToken()
        {
            return Auth(httpProvider.User.RefreshToken);
        }

        void UpdateOAuthInfo(Response response)
        {
            Dictionary<string, string> responseObj;
            responseObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Text);

            httpProvider.User.Id = responseObj["user_id"];
            httpProvider.User.AccessToken = responseObj["access_token"];

            double accessExp = double.Parse(responseObj["expires_in"]) / AccessExpirationDivisor;
            httpProvider.User.AccessExpiry = DateTime.Now.AddSeconds(accessExp);

            httpProvider.User.RefreshToken = responseObj["refresh_token"];
            httpProvider.User.RefreshExpiry = DateTime.Now.AddSeconds(RefreshExpiration * 24 * 3600);

            Storage.Save(httpProvider.User);

			Eventbus.Instance.FireEvent<AuthSuccessEvent>(new AuthSuccessEvent(httpProvider.User));
        }

        public void Dispose()
        {
            StopService();
        }
    }
}