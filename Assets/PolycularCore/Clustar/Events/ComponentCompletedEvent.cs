namespace Polycular.Clustar
{
	public class ComponentCompletedEvent : EventBase
	{
		public IGraphComponent Component { get; set; }


		public ComponentCompletedEvent(IGraphComponent component)
		{
			this.Component = component;
		}
	}
}