using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Polycular.Collections
{
	public class Graph<T> : IEnumerable<GraphNode<T>> where T : class
	{
		GraphNode<T> m_first;

		public GraphNode<T> First
		{
			get
			{
				return m_first;
			}

			private set { m_first = value; }
		}


		public Graph(GraphNode<T> first)
		{
			First = first;
		}

		public Graph(T first)
		{
			First = new GraphNode<T>(first);
		}
		
		public void AddDirectedEdge(T from, T to)
		{
			var vertices = CreateVertices(from, to);
			AddDirectedEdge(vertices.First, vertices.Second);
		}

		public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to)
		{
			from.Outgoing.Add(to);
			to.Ingoing.Add(from);
		}

		public void AddUndirectedEdge(T from, T to)
		{
			var vertices = CreateVertices(from, to);
			AddUndirectedEdge(vertices.First, vertices.Second);
		}

		public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to)
		{
			from.Ingoing.Add(to);
			from.Outgoing.Add(to);

			to.Ingoing.Add(from);
			to.Outgoing.Add(from);
		}

		Tuple<GraphNode<T>, GraphNode<T>> CreateVertices(T from, T to)
		{
			GraphNode<T> fromNode = null;
			GraphNode<T> toNode = null;

			if (from == First.Value)
				fromNode = First;
			else if (to == First.Value)
				toNode = First;

			fromNode = fromNode ?? new GraphNode<T>(from);
			toNode = toNode ?? new GraphNode<T>(to);

			return new Tuple<GraphNode<T>, GraphNode<T>>(fromNode, toNode);
		}

		IEnumerable<GraphNode<T>> BFS()
		{
			var queue = new Queue<GraphNode<T>>();
			var visited = new List<GraphNode<T>>();
			queue.Enqueue(m_first);

			while (queue.Count != 0)
			{
				var current = queue.Dequeue();
				yield return current;
				
				foreach (var neighbour in current.Outgoing)
				{
					if (!visited.Contains(neighbour))
					{
						queue.Enqueue(neighbour);
					}
				}
			}
		}

		public IEnumerator<GraphNode<T>> GetEnumerator()
		{
			foreach (var node in BFS())
				yield return node;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public GraphNode<T> Find(T value)
		{
			foreach (var node in BFS())
			{
				if (node.Value == value)
					return node;
			}
			return null;
		}

		public GraphNode<T> Find(T value, IComparer<T> comparer)
		{
			foreach (var node in BFS())
			{
				if (comparer.Compare(node.Value, value) == 0)
					return node;
			}
			return null;
		}

		string DOTString()
		{
			string gvString = "digraph G {\n";

			foreach (var node in BFS())
			{
				gvString += "\t" + node.DOTSelf() + "\n";
				gvString += "\t" + node.DOTEdges() + "\n";
			}

			gvString += "}";
			return gvString;
		}

		// Creates a png representation of the graph.
		public bool CreateGVImage(string fileName)
		{
#if !UNITY_EDITOR_WIN
			UnityEngine.Debug.LogWarning("Producing a Graphviz Image is currently only supported under Windows.");
			return false;
#else
			string dot = DOTString();
			string path = Application.dataPath + "/PolycularCore/Clustar/GraphViz/";
			string outPath = "out/" + fileName;

			using (var file = File.Open(path + "out/graph.gv", FileMode.Create))
			{
				var rawDot = Encoding.UTF8.GetBytes(dot);
				file.Write(rawDot, 0, rawDot.Length);
			}

			Process program = new Process();

			program.StartInfo.FileName = path + "main/dot.exe";
			program.StartInfo.WorkingDirectory = path;
			program.StartInfo.Arguments = "-Tpng ./out/graph.gv -o " + outPath;  

			program.Start();


			UnityEngine.Debug.Log("Graph image created at: " + path + outPath);
			return true;
#endif
		}
	}	
}

