using JSONApi.Attributes;
using JSONApi.Models;

namespace EcoGotchi.Models
{
	public class Character : IModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[AttributeAttribute]
		public int Comfort { get; set; }

		[AttributeAttribute]
		public int Health { get; set; }

		[AttributeAttribute]
		public int Credits { get; set; }
	}
}