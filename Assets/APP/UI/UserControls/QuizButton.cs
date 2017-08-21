using Noesis;
using Polycular;

namespace EcoGotchi.UI
{
	[UserControlSource("Assets/APP/UI/UserControls/QuizButton.xaml")]
	public class QuizButton : UserControl
	{
		const string NameBtnSel = "Btn_Sel";
		const string NameReBg = "Re_Bg";
		const string NameTbTxt = "Tb_Txt";
		const string NameElSel = "El_Sel";

#pragma warning disable 0414
		static DependencyProperty BrushColorDefaultProperty = DependencyProperty.Register("BrushColorDefault", typeof(Brush), typeof(QuizButton), new PropertyMetadata(null));
		static DependencyProperty BrushColorSelProperty = DependencyProperty.Register("BrushColorSel", typeof(Brush), typeof(QuizButton), new PropertyMetadata(null));
		static DependencyProperty BrushColorWrongProperty = DependencyProperty.Register("BrushColorWrong", typeof(Brush), typeof(QuizButton), new PropertyMetadata(null));
		static DependencyProperty BrushColorRightProperty = DependencyProperty.Register("BrushColorRight", typeof(Brush), typeof(QuizButton), new PropertyMetadata(null));
#pragma warning restore 0414

		public Brush BrushColorDefault
		{
			get { return (Brush)GetValue(BrushColorDefaultProperty); }
		}

		public Brush BrushColorSel
		{
			get { return (Brush)GetValue(BrushColorSelProperty); }
		}

		public Brush BrushColorWrong
		{
			get { return (Brush)GetValue(BrushColorWrongProperty); }
		}

		public Brush BrushColorRight
		{
			get { return (Brush)GetValue(BrushColorRightProperty); }
		}

		Button m_btnSel;
		Rectangle m_reBg;
		TextBlock m_tbTxt;
		Ellipse m_elSel;


		public QuizButton()
		{; }

		public void OnPostInit()
		{
			m_btnSel = FindName(NameBtnSel) as Button;
			m_reBg = FindName(NameReBg) as Rectangle;
			m_tbTxt = FindName(NameTbTxt) as TextBlock;
			m_elSel = FindName(NameElSel) as Ellipse;

			Eventbus.Instance.RemoveListener(this);
			m_btnSel.Click += ((sender, args) =>
			{
				Eventbus.Instance.FireEvent<ButtonTappedEvent>(new ButtonTappedEvent(ButtonTappedEvent.ButtonType.ANSWER, this.Name));
			});
		}

		public void Reset()
		{
			SetSelectionIndicatorActive(false);
			m_reBg.Fill = BrushColorDefault;
		}

		public void SetText(string txt)
		{
			m_tbTxt.Text = txt;
		}

		public void SetSelectedState(bool sel)
		{
			if (sel)
				m_reBg.Fill = BrushColorSel;
			else
				m_reBg.Fill = BrushColorDefault;
		}

		public void SetSelectionIndicatorActive(bool active)
		{
			m_elSel.Visibility = NoesisUtil.BoolToVis(active);
		}

		public void SetWrong()
		{
			m_reBg.Fill = BrushColorWrong;
		}

		public void SetRight()
		{
			m_reBg.Fill = BrushColorRight;
		}
	}
}
