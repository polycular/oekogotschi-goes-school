using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BestHTTP;
using Http.Exceptions;
using RSG;

namespace Http.Clients
{
	public class BestHttpClient : IHttpClient
	{
		public BestHttpClient()
		{
			// comment in to activate dev proxy
			// HTTPManager.Proxy = new HTTPProxy(new Uri("http://192.168.1.157:8888"));
		}

		HTTPMethods GetMethodFromString(string method)
		{
			if (method == "GET")
				return HTTPMethods.Get;
			else if (method == "POST")
				return HTTPMethods.Post;
			else if (method == "DELETE")
				return HTTPMethods.Delete;
			else if (method == "HEAD")
				return HTTPMethods.Head;
			else if (method == "PATCH")
				return HTTPMethods.Patch;
			else if (method == "PUT")
				return HTTPMethods.Put;
			else
				throw new NotSupportedException(string.Format("The string {0} is not a valid http method", method));
		}

		HTTPRequest CreateMessage(Request request)
		{
			var method = GetMethodFromString(request.Method);
			var message = new HTTPRequest(request.Url, method);

			foreach (KeyValuePair<string, string> header in request.Headers)
			{
				message.SetHeader(header.Key, header.Value);
			}

			if (request.Body != null)
			{
				var reader = new StreamReader(request.Body, Encoding.UTF8);
				var content = reader.ReadToEnd();
				message.RawData = Encoding.UTF8.GetBytes(content);
			}

			return message;
		}

		public Promise<Response> Fetch(Request request)
		{
			var promise = new Promise<Response>();
			var message = CreateMessage(request);

			message.Callback = (req, res) =>
			{
				if (res == null)
				{
					promise.Reject(new TimeoutException("http request <b><color=red>failed</color></b>"));
					return;
				}

				if (res.IsSuccess)
				{
					var response = new Response();
					response.Status = res.StatusCode;
					response.Text = res.DataAsText;

					promise.Resolve(response);
				}
				else
				{
					var exception = new HttpException(string.Format("http request <b><color=red>failed</color></b> with <b><color=red>code {0}</color></b> and message {1}", res.StatusCode, res.Message));
					exception.StatusCode = res.StatusCode;

					promise.Reject(exception);
				}
			};
			message.Send();
			return promise;
		}
	}
}
