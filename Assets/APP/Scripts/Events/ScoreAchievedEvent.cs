using Polycular;
using UnityEngine;

namespace EcoGotchi
{
	class ScoreAchievedEvent : EventBase
	{
		public float MappedScore { get; private set; }

		/// <summary> score will be mapped to a range of 1 - 5. </summary>
		public ScoreAchievedEvent(float score, float minVal, float MaxVal)
		{
			if (score <= minVal)
				MappedScore = 1f;
			else if (score >= MaxVal)
				MappedScore = 5f;
			else
				MappedScore = Mathf.Ceil((score - minVal) / (MaxVal - minVal));
		}

		/// <summary> Use this if your score is already normalized to 1 - 5. </summary>
		public ScoreAchievedEvent(int scoreBetweenOneAndFive)
		{ 
			MappedScore = scoreBetweenOneAndFive;
		}
	}
}
