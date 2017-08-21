using EcoGotchi.Models;
using EcoGotchi.UI;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;

namespace EcoGotchi.Behaviours
{
	public class TutorialBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public NoesisGuiController GuiController;
		public TutorialView View;
		
		Component m_rawComponent
		{
			get
			{
				return Component as Component;
			}
		}

		void OnEnable()
		{
			View.NextButtonClicked -= OnNextClicked;
			View.NextButtonClicked += OnNextClicked;
			View.SetNextButtonActive(true);
			GuiController.SetViewVisible(View);
		}

		void OnNextClicked()
		{
			NotifyComponentFinished();
		}

		void OnDisable()
		{
			View.SetNextButtonActive(false);
			Eventbus.Instance.RemoveListener(this);
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}
	}
}
