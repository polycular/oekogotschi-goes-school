using Flurl;
using Polycular.ResourceProvider;
using RSG;

namespace Http
{
    public static class HttpUtilities
	{
		public static IPromise CheckConnectivity(HttpResourceProvider provider)
		{
			Promise promise = new Promise();

			Url url = new Url(provider.ServerConfig.UrlBase);
			url.AppendPathSegment(provider.ServerConfig.ConnectivityEndpoint);

			provider.Get(url, ContentType.JSON, false, false)
				.Then(response => promise.Resolve())
				.Catch(ex => promise.Reject(ex));

			return promise;
		}
	}
}
