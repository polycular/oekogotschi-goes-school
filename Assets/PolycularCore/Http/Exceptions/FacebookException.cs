using System;
using System.Runtime.Serialization;

namespace Http.Exceptions
{
	public class FacebookException : Exception
	{
		public FacebookException()
		{
		}

		public FacebookException(string message) : base(message)
		{
		}

		public FacebookException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FacebookException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
