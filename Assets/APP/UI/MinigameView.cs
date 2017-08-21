using Noesis;

namespace EcoGotchi.UI
{
	public class MinigameView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Minigame.xaml"; }
		}

		Grid m_loading;
		public Grid Loading
		{
			get { return m_loading ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Loading"); }
		}
	}
}