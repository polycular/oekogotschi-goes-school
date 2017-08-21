using RSG;

namespace Http.Clients
{
	public interface IHttpClient
    {
        Promise<Response> Fetch(Request request);
    }
}
