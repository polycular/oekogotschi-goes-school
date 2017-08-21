using System;
using System.Runtime.Serialization;

namespace Http.Exceptions
{
	public class UserLoggedOutException : Exception
	{

		public UserLoggedOutException()
		{
		}

		public UserLoggedOutException(string message) : base(message)
		{
		}

		public UserLoggedOutException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UserLoggedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
