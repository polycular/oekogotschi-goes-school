using System.Collections.Generic;
using JSONApi;
using JSONApi.Attributes;
using JSONApi.Models;
using Polycular.Clustar;

namespace EcoGotchi.Models
{
	public class Component: IModel, IGraphComponent
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Type { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[AttributeAttribute]
		public int OrderId { get; set; }

		[AttributeAttribute]
		public string Hint { get; set; }

		[AttributeAttribute]
		public int PageNumber { get; set; }

		[AttributeAttribute]
		public string Markername { get; set; }

		[AttributeAttribute]
		public List<string> Texts { get; set; }

		[AttributeAttribute]
		public int Score { get; set; }

		[AttributeAttribute]
		public string Decision { get; set; }

		[AttributeAttribute]
		public string Completed { get; set; }

		[AttributeAttribute]
		public int CurrentQuizIdx { get; set; }

		[AttributeAttribute]
		public string Topic { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Consequence> Consequences { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Quiz> Quizzes { get; set; }

		[Relationship(RelationshipType.BELONGS_TO)]
		public Consequence ChosenConsequence { get; set; }


		public override string ToString()
		{
			return string.Format("{0} ({1})", Type, Id);
		}
	}
}