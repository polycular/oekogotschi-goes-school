using System.Collections.Generic;
using JSONApi;
using JSONApi.Attributes;

namespace Polycular.Clustar.ApiModels
{
	public class Project : JSONApi.Models.IModel
	{
		[AttributeAttribute]
		public string Id { get; set; }

		[AttributeAttribute]
		public string Name { get; set; }

		[Relationship(RelationshipType.HAS_MANY)]
		public List<Campaign> Campaigns { get; set; }
	}
}