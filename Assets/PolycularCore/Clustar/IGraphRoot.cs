using System.Collections.Generic;

namespace Polycular.Clustar
{
	public interface IGraphRoot
	{
		IGraphNode First { get; }

		IGraphNode Current { get; set; }

		List<IGraphNode> Nodes { get; }
	}
}