using EcoGotchi.Models;
using EcoGotchi.UI;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;

namespace EcoGotchi.Behaviours
{
	public class NextpageBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;
		public NoesisGuiController GuiController;
		public NextpageView View;
		
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

			GuiController.SetViewVisible(View);
			View.SetContent(m_rawComponent.Hint);
		}

		void OnNextClicked()
		{
			NotifyComponentFinished();
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}
	}
}
