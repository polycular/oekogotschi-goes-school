using System.Linq;
using EcoGotchi.Models;
using EcoGotchi.UI;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;

namespace EcoGotchi.Behaviours
{
	public class DecisionBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public NoesisGuiController GuiController;
		public DecisionView View;

		public float DelayAfterDecision;

		Component m_rawComponent
		{
			get
			{
				return Component as Component;
			}
		}

		int m_selectedDecisionIdx;


		void OnEnable()
		{
			m_selectedDecisionIdx = -1;
			View.Question.Text = m_rawComponent.Decision;

			if (m_rawComponent.Consequences != null)
			{
				var consequences = m_rawComponent.Consequences.Select(c => c as IConsequence).ToList();
				View.InitDecision(consequences);
			}

			Eventbus.Instance.AddListener<ButtonTappedEvent>(OnButtonTappedHandler, this);
			GuiController.SetViewVisible(View);
		}

		void OnDisable()
		{
			Eventbus.Instance.RemoveListener(this);
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}

		void OnButtonTappedHandler(EventBase ev)
		{
			var bte = ev as ButtonTappedEvent;

			if (bte.BtnType == ButtonTappedEvent.ButtonType.ANSWER)
			{
				int len = DecisionView.BaseNameDecisionBtn.Length;
				m_selectedDecisionIdx = int.Parse(bte.BtnName.Remove(0, len));

				View.SelectDecision(m_selectedDecisionIdx);
			}
			else if (bte.BtnType == ButtonTappedEvent.ButtonType.CONTINUE)
			{
				Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);
				Eventbus.Instance.FireEvent<DecisionMadeEvent>(new DecisionMadeEvent(m_rawComponent.Consequences[m_selectedDecisionIdx]));
				NotifyComponentFinished();
			}
		}
	}
}