using Polycular;
using UnityEngine;

namespace EcoGotchi
{
	public class ARTexReadyEvent : EventBase
	{
		public Texture ArTex { get; private set; }


		public ARTexReadyEvent(Texture arTex)
		{
			this.ArTex = arTex;
		}

	}
}