using Polycular;

namespace EcoGotchi.UI
{
	public class ButtonTappedEvent : EventBase
	{
		public enum ButtonType
		{
			TEXT,
			ANSWER,
			CONTINUE
		}

		public ButtonType BtnType { get; private set; }
		public string BtnName { get; private set; }


		public ButtonTappedEvent(ButtonType btnType)
		{
			this.BtnType = btnType;
		}

		public ButtonTappedEvent(ButtonType btnType, string btnName)
		{
			this.BtnType = btnType;
			this.BtnName = btnName;
		}
	}
}