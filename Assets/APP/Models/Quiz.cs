using System.Collections.Generic;
using JSONApi.Attributes;
using JSONApi.Models;

namespace EcoGotchi.Models
{
	public class Quiz : IModel
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Question { get; set; }

		[AttributeAttribute]
		public List<string> Answers { get; set; }

		[AttributeAttribute]
		public List<int> Correct { get; set; }

		[AttributeAttribute]
		public int Points { get; set; }

		[AttributeAttribute]
		public List<string> Chosen { get; set; }
	}
}