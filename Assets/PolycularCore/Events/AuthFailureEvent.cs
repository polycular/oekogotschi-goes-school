using System;

namespace Polycular
{
	public class AuthFailureEvent : EventBase
	{
		public override string ToString()
		{
			return "Failure! At Time: " + DateTime.Now;
		}
	}
}
