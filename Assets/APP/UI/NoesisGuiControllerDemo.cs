using Polycular.Utilities;
using UnityEngine;

namespace EcoGotchi.UI
{
	public class NoesisGuiControllerDemo : MonoBehaviour
	{
		public NoesisGuiController GuiController;

		[Button(true, "SetIntroVisible", "Set Intro visible")]
		public bool SetIntroVis;

		[Button(true, "SetArOverlayVisible", "Set ArOverlay visible")]
		public bool SetArOverlayVis;

		[Button(true, "SetMultiquizVisible", "Set Quiz visible")]
		public bool SetQuizVis;

		[Button(true, "SetDecisionVisible", "Set Decision visible")]
		public bool SetDecisionVis;

		[Button(true, "SetEndVisible", "Set End visible")]
		public bool SetEndVis;


		void Start()
		{

		}

		void Update()
		{

		}

		void SetIntroVisible()
		{
			GuiController.SetViewVisible<IntroView>();
		}

		void SetArOverlayVisible()
		{
			GuiController.SetViewVisible<ArOverlayView>();
		}

		void SetMultiquizVisible()
		{
			GuiController.SetViewVisible<QuizView>();
		}

		void SetDecisionVisible()
		{
			GuiController.SetViewVisible<DecisionView>();
		}

		void SetEndVisible()
		{
			GuiController.SetViewVisible<EndView>();
		}
	}
}