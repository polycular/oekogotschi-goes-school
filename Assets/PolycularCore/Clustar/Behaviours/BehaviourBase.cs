using System.Collections.Generic;
using UnityEngine;

namespace Polycular.Clustar.Behaviours
{
	public class BehaviourBase : MonoBehaviour, IBehaviour
	{
		public event GraphController.BehaviourTaskFinishedHandler OnBehaviourTaskFinished;
		public List<IBehaviour> ActiveBehaviours { get; set; }

		public IGraphComponent Component { get; set; }
		public string NodeName { get; set; }


		protected void Notify(IBehaviour behaviour)
		{
			if(OnBehaviourTaskFinished != null)
			{
				OnBehaviourTaskFinished(behaviour);
			}
		}

		protected T GetActiveBehaviour<T>() where T : BehaviourBase
		{
			T match = ActiveBehaviours.Find(beh => beh is T) as T;
			return match;
		}
	}
}