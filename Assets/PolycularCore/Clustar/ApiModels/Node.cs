using System.Collections.Generic;
using System.Linq;
using JSONApi;
using JSONApi.Attributes;
using JSONApi.Models;

namespace Polycular.Clustar.ApiModels
{
	public class Node : IModel, IGraphNode
	{
		[AttributeAttribute]
		public string Id { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[AttributeAttribute]
		public int CurrentOrderId { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Component> Components { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Transition> Transitions { get; set; }


		List<IGraphComponent> IGraphNode.Components
		{
			get
			{
				return Components.Select(entry => entry as IGraphComponent).ToList();
			}
		}

		List<IGraphTransition> IGraphNode.Transitions
		{
			get
			{
				return Transitions.Select(entry => entry as IGraphTransition).ToList();
			}
		}
	}
}