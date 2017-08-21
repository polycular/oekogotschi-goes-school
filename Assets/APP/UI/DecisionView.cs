using System.Collections.Generic;
using EcoGotchi.Models;
using Noesis;

namespace EcoGotchi.UI
{
	public class DecisionView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Decision.xaml"; }
		}

		public const string BaseNameDecisionBtn = "Uc_DecisionBtn_";

		TextBlock m_Question;
		public TextBlock Question
		{
			get { return m_Question ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Question"); }
		}

		Grid m_Confirm;
		public Grid Confirm
		{
			get { return m_Confirm ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Confirm"); }
		}

		List<DecisionButton> m_decisionBtns;


		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();
		}

		public override void FetchElements()
		{
			m_decisionBtns = new List<DecisionButton>();

			int btnCount = 3;
			string baseNameFormat = "{0}{1}";

			for (int i = 0; i < btnCount; i++)
			{
				string name = string.Format(baseNameFormat, BaseNameDecisionBtn, i);
				var decBtn = NoesisUtil.FetchElement<DecisionButton>(Root, name);
				m_decisionBtns.Add(decBtn);
			}
		}

		public void InitDecision(List<IConsequence> consequences)
		{
			ResetDecisions();
			SetConfirmActive(false);

			for (int i = 0; i < m_decisionBtns.Count; i++)
			{
				if (i > consequences.Count - 1)
					return;

				m_decisionBtns[i].SetText(consequences[i].Option);
			}
		}

		public void SelectDecision(int idx)
		{
			ResetDecisions();
			m_decisionBtns[idx].SetSelected();
			SetConfirmActive(true);
		}

		void SetConfirmActive(bool active)
		{
			Confirm.Visibility = NoesisUtil.BoolToVis(active);
		}

		void ResetDecisions()
		{
			m_decisionBtns.ForEach(btn => btn.Reset());
		}
	}
}