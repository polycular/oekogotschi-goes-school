namespace Polycular.Clustar.Behaviours
{
	public interface IBehaviour
	{
		string NodeName { get; set; }
		IGraphComponent Component { get; set; }
		event GraphController.BehaviourTaskFinishedHandler OnBehaviourTaskFinished;
	}
}