using JSONApi.Attributes;
using JSONApi.Models;

namespace Polycular.Clustar.ApiModels
{
	public class Transition : IModel, IGraphTransition
	{
		[AttributeAttribute]
		public string Id { get; set; }


		[AttributeAttribute]
		public string Type { get; set; }


		[AttributeAttribute]
		public string Data { get; set; }
	}
}