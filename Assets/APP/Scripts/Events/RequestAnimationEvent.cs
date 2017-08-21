using UnityEngine;
using Polycular;

namespace EcoGotchi
{
	public class RequestAnimationEvent : EventBase
	{
		public AnimType Animation { get; private set; }

		public RequestAnimationEvent(AnimType animation)
		{
			Animation = animation;
		}
	}
}
