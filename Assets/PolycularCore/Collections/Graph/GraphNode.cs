using System.Collections.Generic;

namespace Polycular.Collections
{
	public class GraphNode<T> 
	{
		static int labelID = 0;
		public static GVNodeContentCreator NodeContentCreator;

		public delegate string GVNodeContentCreator(GraphNode<T> node);

		List<GraphNode<T>> m_ingoing = new List<GraphNode<T>>();
		List<GraphNode<T>> m_outgoing = new List<GraphNode<T>>();
		T m_value;

		public GraphNode(T value)
		{
			this.m_value = value;
			Label = labelID;
			labelID++;
		}

		public List<GraphNode<T>> Ingoing
		{
			get
			{
				return m_ingoing;
			}

			private set {}
		}

		// Used for unique ids when drawing a GraphViz Graph
		public int Label { get; private set; }

		public List<GraphNode<T>> Outgoing
		{
			get
			{
				return m_outgoing;
			}

			private set {}
		}

		public T Value
		{
			get
			{
				return m_value;
			}
			private set {}
		}

		public string DOTSelf()
		{
			if (NodeContentCreator != null)
			{
				return Label + "[shape=box,label=\"" + NodeContentCreator(this) + "\"]";
			}
			else
			{
				return Label + "[shape=box,label=\"" + m_value.ToString() + "\"]";
			}
		}

		public string DOTEdges()
		{
			string edges = "";

			foreach (var edge in Outgoing)
			{
				edges += Label + " -> " + edge.Label;
			}

			return edges;
		}
	}
}