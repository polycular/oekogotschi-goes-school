using System.Collections.Generic;
using System.Linq;
using JSONApi;
using JSONApi.Attributes;
using JSONApi.Models;
using Polycular.Clustar;

namespace EcoGotchi.Models
{
	public class Node : IModel, IGraphNode
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Component> Components { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Transition> Transitions { get; set; }

		[AttributeAttribute]
		public int CurrentOrderId { get; set; }


		List<IGraphComponent> IGraphNode.Components
		{
			get
			{
				if (Components == null)
					return null;
				else
					return Components.Select(entry => entry as IGraphComponent).ToList();
			}
		}

		List<IGraphTransition> IGraphNode.Transitions
		{
			get
			{
				if (Transitions == null)
					return null;
				else
					return Transitions.Select(entry => entry as IGraphTransition).ToList();
			}
		}
	}
}