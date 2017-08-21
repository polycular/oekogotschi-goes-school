using EcoGotchi.Models;
using Polycular;

namespace EcoGotchi.UI
{
	public class DecisionMadeEvent : EventBase
	{
		public IConsequence ChosenConsequence { get; private set; }


		public DecisionMadeEvent(IConsequence con)
		{
			this.ChosenConsequence = con;
		}
	}
}