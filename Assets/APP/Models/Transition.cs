using JSONApi.Attributes;
using JSONApi.Models;
using Polycular.Clustar;

namespace EcoGotchi.Models
{
	public class Transition : IModel, IGraphTransition
	{
		public string Id { get; set; }

		[AttributeAttribute]
		public string Type { get; set; }

		[AttributeAttribute]
		public string Data { get; set; }
	}
}