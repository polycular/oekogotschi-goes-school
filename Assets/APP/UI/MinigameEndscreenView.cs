using Noesis;

namespace EcoGotchi.UI
{
	public class MinigameEndscreenView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "MinigameEndscreen.xaml"; }
		}

		TextBlock m_minigamePoints;
		public TextBlock MinigamePoints
		{
			get
			{
				if (m_minigamePoints == null)
					m_minigamePoints = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_MinigamePoints");

				return m_minigamePoints;
			}
		}

		TextBlock m_ecoPoints;
		public TextBlock EcoPoints
		{
			get
			{
				if (m_ecoPoints == null)
					m_ecoPoints = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_EcoPoints");

				return m_ecoPoints;
			}
		}

		public MinigameInfo MinigameInfo;


		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();
		}

		public override void EstablishBindings()
		{
			// Minigame Score
			Binding bMinigameScore = new Binding();
			bMinigameScore.Source = MinigameInfo;
			bMinigameScore.Path = new PropertyPath("Score");
			bMinigameScore.Mode = BindingMode.OneWay;
			bMinigameScore.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			MinigamePoints.SetBinding(TextBlock.TextProperty, bMinigameScore);
		}
	}
}