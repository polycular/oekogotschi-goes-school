namespace Polycular.Clustar
{
	public interface IGraphComponent
	{
		string Id { get; }
		string Type { get; }
		int OrderId { get; }
	}
}