using Noesis;

namespace EcoGotchi.UI
{
	public interface INoesisView
	{
		string XamlFileName { get; }
		bool IsOverlay { get; }
		FrameworkElement Root { get; set; }

		void FetchElements();
		void RegisterEventHandler();
		void EstablishBindings();
	}
}