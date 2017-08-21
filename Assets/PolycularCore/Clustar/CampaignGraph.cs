using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Polycular.Collections;

namespace Polycular.Clustar
{
	public class CampaignGraph
	{
		Graph<IGraphNode> _graph;


		public CampaignGraph(IGraphNode first, List<IGraphNode> nodes)
		{
			var graphNodes = nodes.Select(node => new GraphNode<IGraphNode>(node)).ToList();
			var firstNode = graphNodes.Find(n => n.Value.Id == first.Id);
			_graph = new Graph<IGraphNode>(firstNode);

			foreach (var current in graphNodes)
			{
				if (current.Value.Transitions == null)
					continue;

				foreach (var trans in current.Value.Transitions)
				{
					/*
						This currently only works with "next" transitions
						and will have to be adjusted once new transitions 
						are introduced.
					*/

					JObject data = JObject.Parse(trans.Data);
					string nextNodeId = data["target"].ToString();

					var neighbour = graphNodes.Find(n => n.Value.Id == nextNodeId);
					_graph.AddDirectedEdge(current, neighbour);
				}
			}
		}

		public List<IGraphNode> GetNextNode(IGraphNode current)
		{
			foreach (var node in _graph)
			{
				if (node.Value == current)
					return node.Outgoing.Select(n => n.Value).ToList();
			}

			return null;
		}

		/// <summary>Iterates over the graph using BFS, returning the hash of
		/// the node's concatenated Ids.</summary>
		public override int GetHashCode()
		{
			string total = string.Empty;
			foreach (var node in _graph)
			{
				total += node.Value.Id;
			}

			return total.GetHashCode();
		}

		public bool Plot(string fileName)
		{
			return _graph.CreateGVImage(fileName);
		}
	}	
}