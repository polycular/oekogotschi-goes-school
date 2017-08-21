using Noesis;
using Polycular;

namespace EcoGotchi.UI
{
	[UserControlSource("Assets/APP/UI/UserControls/DecisionButton.xaml")]
	public class DecisionButton : UserControl
	{
		const string NameBtnDecision = "Btn_Decision";
		const string NameTbDecision = "Tb_Decision";
		const string NameReBg = "Re_Bg";

#pragma warning disable 0414
		static DependencyProperty BrushBgProperty = DependencyProperty.Register("BrushBg", typeof(Brush), typeof(DecisionButton));
		static DependencyProperty BrushSelProperty = DependencyProperty.Register("BrushSel", typeof(Brush), typeof(DecisionButton));
#pragma warning restore 0414

		public Brush BrushBg
		{
			get { return (Brush)GetValue(BrushBgProperty); }
		}

		public Brush BrushSel
		{
			get { return (Brush)GetValue(BrushSelProperty); }
		}

		Button m_Btn;
		TextBlock m_Text;
		Rectangle m_Bg;


		public DecisionButton()
		{; }

		public void OnPostInit()
		{
			m_Btn = FindName(NameBtnDecision) as Button;
			Eventbus.Instance.RemoveListener(this);
			m_Btn.Click += ((sender, args) =>
			{
				Eventbus.Instance.FireEvent<ButtonTappedEvent>(new ButtonTappedEvent(ButtonTappedEvent.ButtonType.ANSWER, this.Name));
			});

			m_Text = FindName(NameTbDecision) as TextBlock;
			m_Bg = FindName(NameReBg) as Rectangle;
		}


		public void SetText(string txt)
		{
			m_Text.Text = txt;
		}

		public void SetSelected()
		{
			m_Bg.Fill = BrushSel;
		}

		public void Reset()
		{
			m_Bg.Fill = BrushBg;
		}
	}
}