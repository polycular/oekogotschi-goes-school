using Noesis;

namespace EcoGotchi.UI
{
	public class IntroView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Intro.xaml"; }
		}

		TextBlock m_introTxtBlock;
		public TextBlock IntroTxtBlock
		{
			get { return m_introTxtBlock ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Intro"); }
		}


		TextBlock m_topicTxtBlock;
		public TextBlock TopicTxtBlock
		{
			get { return m_topicTxtBlock ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Topic"); }
		}

		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();
		}
	}
}