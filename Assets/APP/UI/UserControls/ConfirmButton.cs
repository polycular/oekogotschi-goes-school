using Noesis;
using Polycular;

namespace EcoGotchi.UI
{
	[UserControlSource("Assets/APP/UI/UserControls/ConfirmButton.xaml")]
	public class ConfirmButton : UserControl
	{
		const string NameBtn = "Btn_Next";
		const string NameReBg = "Re_Bg";

		const string NameResMainBg = "Scb_Blue3";
		const string NameResPressedBg = "Scb_Blue4";

		Button m_btn;
		Rectangle m_bg;

		SolidColorBrush m_mainBg;
		SolidColorBrush m_pressedBg;


		public ConfirmButton()
		{; }

		public void OnPostInit()
		{
			m_bg = FindName(NameReBg) as Rectangle;

			m_mainBg = FindResource(NameResMainBg) as SolidColorBrush;
			m_pressedBg = FindResource(NameResPressedBg) as SolidColorBrush;

			m_btn = FindName(NameBtn) as Button;
			m_btn.Click += ((sender, args) =>
			{
				Eventbus.Instance.FireEvent<ButtonTappedEvent>(new ButtonTappedEvent(ButtonTappedEvent.ButtonType.CONTINUE));
			});

			m_btn.MouseDown += ((sender, args) =>
			{
				m_bg.Fill = m_pressedBg;
			});

			m_btn.MouseUp += ((sender, args) =>
			{
				m_bg.Fill = m_mainBg;
			});
		}
	}
}