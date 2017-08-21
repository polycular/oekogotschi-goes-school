using System.Collections.Generic;

namespace EcoGotchi.Models
{
	public interface IConsequence
	{
		string Id { get; }

		int Comfort { get; }

		int Health { get; }

		int Credits { get; }

		string Option { get; }

		string Header { get; }

		List<string> Texts { get; }
	}
}