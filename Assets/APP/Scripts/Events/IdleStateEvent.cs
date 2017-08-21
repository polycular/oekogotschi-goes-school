using Polycular;

namespace EcoGotchi
{
	public class IdleStateEvent : EventBase
	{
		public IdleState IdleState { get; private set; }

		public IdleStateEvent(IdleState state)
		{
			IdleState = state;
		}
	}
}
