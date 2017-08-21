using System;
using System.Runtime.Serialization;

namespace JSONApi.Exceptions
{ 
	public class UserIsTesterException : Exception
	{
		public UserIsTesterException()
		{
		}

		public UserIsTesterException(string message) : base(message)
		{
		}

		public UserIsTesterException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UserIsTesterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}