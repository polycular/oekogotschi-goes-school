using Polycular.Clustar.Behaviours;


namespace Polycular.Clustar.Behaviour
{
	/*\
	|*| Example class showing how to implement a
	|*| Behaviour to use with Clustar.
	\*/
	[System.Serializable]
	public class ExampleBehaviour : BehaviourBase, IBehaviour
	{
		void OnEnable()
		{
			// Start Logic
		}

		void OnDisable()
		{
			// End Logic
		}
	}
}