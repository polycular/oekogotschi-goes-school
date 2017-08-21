using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Http
{
	public sealed class Request
	{
		public Dictionary<string, string> Headers { get; private set; }

		public Dictionary<string, string> Parameters { get; private set; }

		public Uri Url { get; private set; }

		public String Method { get; private set; }

		public Stream Body { get; private set; }

		public static Request Options(string url)
		{
			return new Request(new Uri(url), Verbs.OPTIONS);
		}

		public static Request Head(string url)
		{
			return new Request(new Uri(url), Verbs.HEAD);
		}

		public static Request Get(string url)
		{
			return new Request(new Uri(url), Verbs.GET);
		}

		public static Request Post(string url)
		{
			return new Request(new Uri(url), Verbs.POST);
		}

		public static Request Put(string url)
		{
			return new Request(new Uri(url), Verbs.PUT);
		}

		public static Request Patch(string url)
		{
			return new Request(new Uri(url), Verbs.PATCH);
		}

		public static Request Delete(string url)
		{
			return new Request(new Uri(url), Verbs.DELETE);
		}

		public Request(Uri url, string method)
		{
			Url = url;
			Method = method;
			Headers = new Dictionary<string, string>();
			Parameters = new Dictionary<string, string>();
		}

		public Request SetHeader(String key, String val)
		{
			Headers.Add(key, val);
			return this;
		}

		public Request SetBody(Dictionary<string, string> parameters)
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.NullValueHandling = NullValueHandling.Ignore;

				serializer.Serialize(sw, parameters);
			}

			string str = sb.ToString();

			Body = new MemoryStream(Encoding.UTF8.GetBytes(str));
			return this;
		}

		public Request SetBody(Stream body)
		{
			Body = body;
			return this;
		}

		public Request SetBody(string body)
		{
			Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
			return this;
		}
	}
}
