using System;
using Noesis;

namespace EcoGotchi.UI
{
	public class ArOverlayView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "ArOverlay.xaml"; }
		}

		public event Action OnHelpButtonClick;

		public HeaderbarView HeaderView;

		Grid m_loading;
		public Grid Loading
		{
			get { return m_loading ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Loading"); }
		}

		Grid m_help;
		public Grid Help
		{
			get { return m_help ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Help"); }
		}

		Button m_helpBtn;
		public Button HelpBtn
		{
			get { return m_helpBtn ?? NoesisUtil.FetchElement<Button>(Root, "Btn_Help"); }
		}

		Grid m_scanPrompt;
		public Grid ScanPrompt
		{
			get { return m_scanPrompt ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_ScanPrompt"); }
		}

		TextBlock m_scanPromptText; 
		public TextBlock ScanPromptText
		{
			get { return m_scanPromptText ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_ScanPrompt"); }
		}

		Grid m_speech;
		public Grid Speech
		{
			get { return m_speech ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Speech"); }
		}

		TextBlock m_speechTxt;
		public TextBlock SpeechText
		{
			get { return m_speechTxt ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Speech"); }
		}

		Grid m_outcome;
		public Grid Outcome
		{
			get { return m_outcome ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Outcome"); }
		}

		TextBlock m_outcomeHeader;
		public TextBlock OutcomeHeader
		{
			get { return m_outcomeHeader ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Outcome_Header"); }
		}

		TextBlock m_outcomeContent;
		public TextBlock OutcomeContent
		{
			get { return m_outcomeContent ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Outcome_Content"); }
		}

		Grid m_coinAR;
		public Grid CoinAR
		{
			get { return m_coinAR ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Coin_AR"); }
		}

		Grid m_coinBlank;
		public Grid CoinBlank
		{
			get { return m_coinBlank ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Coin_Blank"); }
		}

		Grid m_confirmBtn;
		public Grid ConfirmBtn
		{
			get { return m_confirmBtn ?? NoesisUtil.FetchElement<Grid>(Root, "Gr_Confirm"); }
		}

		TextBlock m_tbDescrComfortChange;
		public TextBlock DescrComfortChange
		{
			get { return m_tbDescrComfortChange ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Description_Comfort"); }
		}

		TextBlock m_tbDescrHealthChange;
		public TextBlock DescrHealthChange
		{
			get { return m_tbDescrHealthChange ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Description_Health"); }
		}

		TextBlock m_tbDescrCreditsChange;
		public TextBlock DescrCreditstChange
		{
			get { return m_tbDescrCreditsChange ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Description_Credits"); }
		}

		void Start()
		{
			Init();
		}

		public void Init()
		{
			MainGrid.Width = NoesisUtil.GetWidth();

			HelpBtn.Click -= OnHelpButtonClickedHandler;
			HelpBtn.Click += OnHelpButtonClickedHandler;

			SetOverlayActive(ScanPrompt);
		}

		public void SetDecisionFeedback(string header, string txt)
		{
			OutcomeHeader.Text = header;
			OutcomeContent.Text = txt;
		}
		
		public void SetOverlayActive(Grid overlay)
		{
			if (overlay == ScanPrompt)
			{
				SetCoinActive(CoinAR);
				SetConfirmBtnActive(false);
			}
			else
			{
				SetCoinActive(CoinBlank);
				SetConfirmBtnActive(true);
			}

			Speech.Visibility = NoesisUtil.BoolToVis(Speech == overlay);
			ScanPrompt.Visibility = NoesisUtil.BoolToVis(ScanPrompt == overlay);
			Outcome.Visibility = NoesisUtil.BoolToVis(Outcome == overlay);
		}

		public void SetHelpActive(bool active)
		{
			Help.Visibility = NoesisUtil.BoolToVis(active);
		}

		void SetCoinActive(Grid coin)
		{
			CoinAR.Visibility = NoesisUtil.BoolToVis(coin == CoinAR);
			CoinBlank.Visibility = NoesisUtil.BoolToVis(coin == CoinBlank);
		}

		void SetConfirmBtnActive(bool active)
		{
			ConfirmBtn.Visibility = NoesisUtil.BoolToVis(active);
		}

		void OnHelpButtonClickedHandler(object sender, Noesis.EventArgs args)
		{
			if (OnHelpButtonClick != null)
				OnHelpButtonClick();
		}
	}
}