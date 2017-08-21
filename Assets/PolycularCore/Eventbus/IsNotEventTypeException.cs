using System;
using System.Runtime.Serialization;

namespace Polycular
{
	public class IsNotEventTypeException : Exception
	{
		public IsNotEventTypeException()
		{
		}

		public IsNotEventTypeException(string message) : base(message)
		{
		}

		public IsNotEventTypeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected IsNotEventTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

