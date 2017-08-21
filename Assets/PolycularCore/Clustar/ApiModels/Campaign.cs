using System.Collections.Generic;
using System.Linq;
using JSONApi;
using JSONApi.Attributes;
using JSONApi.Models;

namespace Polycular.Clustar.ApiModels
{
	public class Campaign : IModel, IGraphRoot
	{
		[AttributeAttribute]
		public string Id { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[AttributeAttribute]
		public float Score { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Node> Nodes { get; set; }

		[Relationship(RelationshipType.BELONGS_TO)]
		public Node First { get; set; }

		[Relationship(RelationshipType.BELONGS_TO)]
		public Node Current { get; set; }


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
				return Nodes.Select(entry => entry as IGraphNode).ToList();
			}
		}
	}
}