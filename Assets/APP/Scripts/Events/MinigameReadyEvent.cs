using Polycular;

namespace EcoGotchi
{
	public class MinigameReadyEvent : EventBase
	{
		public IGameDirector GameDirector { get; private set; }


		public MinigameReadyEvent(IGameDirector gd)
		{
			this.GameDirector = gd;
		}
	}
}