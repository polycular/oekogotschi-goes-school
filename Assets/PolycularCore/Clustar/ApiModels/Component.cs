using System.Collections.Generic;
using JSONApi.Attributes;
using JSONApi.Models;

namespace Polycular.Clustar.ApiModels
{
	public class Component : IModel, IGraphComponent
	{
		[AttributeAttribute]
		public string Id { get; set; }

		[AttributeAttribute]
		public string Type { get; set; }

		[AttributeAttribute]
		public int OrderId { get; set; }

		[AttributeAttribute]
		public string Completed { get; set; }

		[AttributeAttribute]
		public int Points { get; set; }

		[AttributeAttribute]
		public float Score { get; set; }

		[AttributeAttribute]
		public int Penalty { get; set; }

		[AttributeAttribute]
		public string Text { get; set; }

		[AttributeAttribute]
		public string Hint { get; set; }

		[AttributeAttribute]
		public bool Used { get; set; }

		[AttributeAttribute]
		public List<string> Texts { get; set; }

		[AttributeAttribute]
		public List<string> Urls { get; set; }

		[AttributeAttribute]
		public double Latitude { get; set; }

		[AttributeAttribute]
		public double Longitude { get; set; }

		[AttributeAttribute]
		public double Radius { get; set; }

		[AttributeAttribute]
		public string Question { get; set; }

		[AttributeAttribute]
		public List<string> Answers { get; set; }

		[AttributeAttribute]
		public List<int> Correct { get; set; }

		[AttributeAttribute]
		public List<int> Chosen { get; set; }

		[AttributeAttribute]
		public string Email { get; set; }

		[AttributeAttribute]
		public bool Participate { get; set; }

		[AttributeAttribute]
		public string[] Uris { get; set; }

		[AttributeAttribute]
		public string[] Assetbundlenames { get; set; }

		[AttributeAttribute]
		public string[] Assetnames { get; set; }

		[AttributeAttribute]
		public string[] Markernames { get; set; }
	}
}