namespace Polycular.Persistance
{
	interface IPersistant
	{
		string SavePath { get; }

		string Serialize();
		void Deserialize(string serialObject);
	}
}