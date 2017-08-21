using System.Collections.Generic;
using System.Linq;
using JSONApi;
using JSONApi.Attributes;
using JSONApi.Models;
using Polycular.Clustar;

namespace EcoGotchi.Models
{
	public class Campaign : IModel, IGraphRoot
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[AttributeAttribute]
		public string State { get; set; }

		[AttributeAttribute]
		public int Score { get; set; }
		
		[Relationship(RelationshipType.BELONGS_TO)]
		public Node First { get; set; }

		[Relationship(RelationshipType.BELONGS_TO)]
		public Node Current { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Node> Nodes { get; set; }

		[Relationship(RelationshipType.BELONGS_TO)]
		public Character Character { get; set; }

		IGraphNode IGraphRoot.First
		{
			get { return First; }
		}

		IGraphNode IGraphRoot.Current
		{
			get { return Current; }
			set { Current = value as Node; }
		}

		List<IGraphNode> IGraphRoot.Nodes
		{
			get
			{
				if (Nodes == null)
					return null;
				else
					return Nodes.Select(entry => entry as IGraphNode).ToList();
			}
		}
	}
}