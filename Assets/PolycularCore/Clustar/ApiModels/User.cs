using JSONApi.Attributes;
using JSONApi.Models;

namespace Polycular.Clustar.ApiModels
{
	public class User : IModel
	{
		[AttributeAttribute]
		public string Id { get; set; }

		[AttributeAttribute]
		public string Email { get; set; }
	}
}
