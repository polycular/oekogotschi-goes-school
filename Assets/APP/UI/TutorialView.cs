using System;
using Noesis;

namespace EcoGotchi.UI
{
	public class TutorialView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Tutorial.xaml"; }
		}

		public event Action NextButtonClicked;
		public event Action ExitButtonClicked;

		public const string NameNextButton = "Btn_Next";
		public const string NameExitButton = "Btn_Exit";

		Button m_nextButton;
		Button m_exitButton;


		public override void FetchElements()
		{
			m_nextButton = NoesisUtil.FetchElement<Button>(Root, NameNextButton);
			m_exitButton = NoesisUtil.FetchElement<Button>(Root, NameExitButton);

			m_nextButton.Visibility = Visibility.Hidden;
			m_exitButton.Visibility = Visibility.Hidden;

			m_nextButton.Click -= OnNextClick;
			m_nextButton.Click += OnNextClick;

			m_exitButton.Click -= OnExitClick;
			m_exitButton.Click += OnExitClick;
		}

		void OnNextClick(object sender, Noesis.EventArgs e)
		{
			NextButtonClicked();
		}

		void OnExitClick(object sender, Noesis.EventArgs e)
		{
			ExitButtonClicked();	
		}

		/// <summary> Enables the button used to signal component finished to TutorialBehaviour. </summary>
		public void SetNextButtonActive(bool active)
		{
			m_nextButton.Visibility = NoesisUtil.BoolToVis(active);
		}

		/// <summary> Enables the button used to exit the View when used as a help overlay. </summary>
		public void SetExitButtonActive(bool active)
		{
			m_exitButton.Visibility = NoesisUtil.BoolToVis(active);
		}
	}
}