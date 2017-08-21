using JSONApi.Attributes;
using JSONApi.Models;
using System.Collections.Generic;

namespace EcoGotchi.Models
{
	public class Consequence : IModel, IConsequence
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Option { get; set; }

		[AttributeAttribute]
		public int Comfort { get; set; }

		[AttributeAttribute]
		public int Health { get; set; }

		[AttributeAttribute]
		public int Credits { get; set; }

		[AttributeAttribute]
		public string Header { get; set; }

		[AttributeAttribute]
		public List<string> Texts { get; set; }
	}
}