using System.Collections.Generic;

namespace Polycular.Clustar
{
	public interface IGraphNode
	{
		string Id { get; }

		string Name { get; }

		int CurrentOrderId { get; set; }

		List<IGraphComponent> Components { get; }

		List<IGraphTransition> Transitions { get; }
	}
}