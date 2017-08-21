using System;
using System.Collections.Generic;
using Facebook.Unity;
using Flurl;
using Http;
using Http.Clients;
using Http.Exceptions;
using Polycular.Persistance;
using RSG;
using UnityEngine;

namespace Polycular.ResourceProvider
{
	public enum ContentType
	{
		JSON,
		PLAIN
	}

	public class HttpResourceProvider
	{
		List<string> testerEmails;

		public User User { get; private set; }

		ServerConfig serverConfig;
		public ServerConfig ServerConfig
		{
			get
			{
				if (serverConfig == null)
				{
					// init for "Clustar V2"
					serverConfig = new ServerConfig
					{
						UrlBase = "https://api-stage.clustar.net/v2/",
						OAuthEndpoint = "oauth/token",
						FbEndpoint = "auth/facebook",
						ConnectivityEndpoint = "ping",
						UserEndpoint = "users"
					};
				}

				return serverConfig;
			}
			private set { }
		}

		OAuth oAuth;
		public OAuth OAuth
		{
			get
			{
				if (oAuth == null)
					oAuth = new OAuth(this, 10, 13, 2);
				return oAuth;
			}
			private set { }
		}

		IHttpClient httpClient;
		RequestQueue requestQueue;

		public HttpResourceProvider()
		{
			httpClient = new BestHttpClient();
			User = new User();

			Storage.Load(User);

			if (User.Id != null)
				OAuth.StartService();

			requestQueue = new RequestQueue(this, 5);
			testerEmails = new List<string>();

			using (var drp = new DiskResourceProvider())
			{
				var res = drp.Read("email_adr_tester.txt");

				if (res.Success)
				{
					var emails = res.ReadContent.Split('\n');
					foreach (var addr in emails)
						testerEmails.Add(addr);
				}
			}
		}

		public Promise RegisterUser(string email, string password)
		{
			return OAuth.Register(email, password)
				.Then(() => LoginUser(email, password)) as Promise;
		}

		public Promise LoginUser(string email, string password)
		{
			User.AuthType = User.AuthenticationType.NATIVE;
			User.Email = email;
			User.Password = password;

			if (testerEmails.Contains(email))
			{
				User.IsTester = true;
			}

			Promise promise;

			if (!User.IsLoaded)
			{
				promise = OAuth.Auth(email, password);
				promise
				.Then(() =>
				{
					Storage.Save(User);
					OAuth.StartService();
				})
				.Catch(ex =>
				{
					Debug.LogWarning(ex);
				});
			}
			else
			{
				promise = new Promise();
				Storage.Save(User);
				promise.Resolve();
			}

			return promise;
		}

		public Promise LoginUserFacebook()
		{
			User.AuthType = User.AuthenticationType.FACEBOOK;

			var prm = new Promise();
			var permissions = new List<string>() { "public_profile", "email" };

			FB.Init(() =>
			{
				FB.LogInWithReadPermissions(permissions, (result) =>
				{
					if (!string.IsNullOrEmpty(result.Error))
					{
						Debug.LogError(result.Error);
						prm.Reject(new FacebookException(result.Error));
						return;
					}

					var aToken = AccessToken.CurrentAccessToken;

					OAuth.AuthFacebook(aToken.UserId, aToken.TokenString)
						.Then(() =>
						{
							Debug.Log("FB Auth succeeded");

							FB.API("/me/?fields=name", HttpMethod.GET, (res) =>
							{
								if (!string.IsNullOrEmpty(res.ResultDictionary["name"].ToString()))
									User.Username = res.ResultDictionary["name"].ToString();
							});

							User.Email = string.Format("{0}@facebook.com", aToken.UserId);

							prm.Resolve();
						})
						.Catch(ex =>
						{
							Debug.Log("FB Auth failed:\n" + ex);
							prm.Reject(ex);
						});
				});
			});

			return prm;
		}

		public Promise<Response> Fetch(Request req)
		{
			return httpClient.Fetch(req);
		}

		public void Reset()
		{
			User = new User();
			requestQueue.Reset();
			OAuth.StopService();
		}

		public Promise<Response> Get(Url uri, ContentType type, bool authorized = true, bool useQueue = true)
		{
			return FetchImpl(uri, type, null, Verbs.GET, authorized, useQueue);
		}

		public Promise<Response> Patch(Url uri, string content, ContentType type, bool authorized = true, bool useQueue = true)
		{
			return FetchImpl(uri, type, content, Verbs.PATCH, authorized, useQueue);
		}

		public Promise<Response> Post(Url uri, string content, ContentType type, bool authorized = true, bool useQueue = true)
		{
			return FetchImpl(uri, type, content, Verbs.POST, authorized, useQueue);
		}

		public Promise<Response> Post(Url uri, Dictionary<string, string> content, ContentType type, bool authorized = true, bool useQueue = true)
		{
			return FetchImpl(uri, type, content, Verbs.POST, authorized, useQueue);
		}

		public Promise<Response> Delete(Url uri, bool authorized = true, bool skipQueue = false)
		{
			throw new NotImplementedException();
		}

		public Promise<Response> FetchImpl(Url uri, ContentType type, object content, string method, bool authorized = true, bool useQueue = true)
		{
			Url finalUri = uri;
			Request req = new Request(new Uri(uri), method);

			if (authorized)
			{
				if (User == null)
				{
					Promise<Response> promise = new Promise<Response>();
					promise.Reject(new UserNotSetException());
					return promise;
				}

				finalUri = GetAuthenticatedRoute(uri);
				req = Request.Get(finalUri);

				bool running = true;

				SetAuth(req)
					.Then(() => running = false)
					.Catch(ex => running = false);

				while (running) ;
			}

			SetContentType(req, type);

			if (content != null)
			{
				if (content is string)
				{
					req.SetBody(content as string);
				}
				else if (content is Dictionary<string, string>)
				{
					req.SetBody(content as Dictionary<string, string>);
				}
			}

			if (useQueue)
			{
				return requestQueue.Enqueue(new QueuedRequest(req));
			}

			return httpClient.Fetch(req);
		}
		
		void SetContentType(Request req, ContentType type)
		{
			string value = string.Empty;

			switch (type)
			{
				case ContentType.JSON:
					value = "application/json";
					break;

				default:
					value = "text/plain";
					break;
			}

			req.SetHeader("content-type", value);
			req.SetHeader("accept", value);
		}

		Promise SetAuth(Request req)
		{
			var promise = new Promise();

			oAuth.GetAccessToken()
				.Then(atoken =>
				{
					req.SetHeader("Authorization", string.Format("{0} {1}", "Bearer", atoken));
					promise.Resolve();
				})
				.Catch(ex => promise.Reject(ex));

			return promise;
		}

		Url GetAuthenticatedRoute(Url url)
		{
			if (string.IsNullOrEmpty(User.Id))
			{
				throw new UserNotSetException("Attempt to send authorized request without being logged in. The authenticated route would therefore be incomplete.");
			}

			// base path
			string basePath = ServerConfig.UrlBase;

			// route
			string routeInclParams = url.ToString().Remove(0, basePath.Length);
			string route = routeInclParams.ToString().Split('?')[0];

			// query params
			var queryParams = url.QueryParams;

			// reconstruct + add auth information
			Url authUrl = new Url(basePath);
			authUrl.AppendPathSegment(ServerConfig.UserEndpoint);
			authUrl.AppendPathSegment(User.Id);
			authUrl.AppendPathSegment(route);
			authUrl.SetQueryParams(queryParams);

			return authUrl;
		}
	}
}