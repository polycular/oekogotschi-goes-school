using Http;

namespace Polycular
{
	public class AuthSuccessEvent : EventBase
	{
		User LoggedInUser { get; set; }

		public AuthSuccessEvent(User user)
		{
			LoggedInUser = user;
		}

		public override string ToString()
		{
			return LoggedInUser.ToString();
		}
	}
}
