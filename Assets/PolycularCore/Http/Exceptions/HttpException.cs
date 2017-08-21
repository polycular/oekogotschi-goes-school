using System;
using System.Runtime.Serialization;


namespace Http.Exceptions
{
	class HttpException : Exception
	{
		public int StatusCode { get; internal set; }


		public HttpException()
		{
		}

		public HttpException(string message) : base(message)
		{
		}

		public HttpException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected HttpException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
