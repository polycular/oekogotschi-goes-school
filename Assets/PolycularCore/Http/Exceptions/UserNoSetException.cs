using System;
using System.Runtime.Serialization;

namespace Http.Exceptions
{
	class UserNotSetException : Exception
	{
		public UserNotSetException()
		{
		}

		public UserNotSetException(string message) : base(message)
		{
		}

		public UserNotSetException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UserNotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}