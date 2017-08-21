using System.Collections.Generic;
using Noesis;
using System;

namespace EcoGotchi.UI
{
	public class NextpageView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Nextpage.xaml"; }
		}

		public event Action NextButtonClicked;
		
		public const string NameNextButton = "Btn_Next";
		public const string NameTbContent = "Tb_Content";

		Button m_nextButton;
		TextBlock m_tbContent;
		
		public override void FetchElements()
		{
			m_nextButton = NoesisUtil.FetchElement<Button>(Root, NameNextButton);
			m_tbContent = NoesisUtil.FetchElement<TextBlock>(Root, NameTbContent);
			
			m_nextButton.Click -= OnNextClick;
			m_nextButton.Click += OnNextClick;
		}

		public void SetContent(string content)
		{
			m_tbContent.Text = content;
		}

		void OnNextClick(object sender, Noesis.EventArgs e)
		{
			NextButtonClicked();
		}
	}
}