using Polycular.Clustar.Behaviours;

namespace Polycular.Clustar
{
	class BehaviourTask
	{
		public IBehaviour Behaviour { get; private set; }
		public IGraphComponent Component { get; private set; }

		public BehaviourTask(IBehaviour beh, IGraphComponent comp)
		{
			Behaviour = beh;
			Component = comp;
		}
	}
}
