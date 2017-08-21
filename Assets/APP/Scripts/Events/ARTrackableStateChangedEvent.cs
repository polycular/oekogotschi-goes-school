using Polycular;

namespace EcoGotchi
{
	public class ARTrackableStateChangedEvent : EventBase
	{
		public string MarkerName { get; private set; }
		public bool IsTracked { get; private set; }


		public ARTrackableStateChangedEvent(bool tracked)
		{
			this.IsTracked = tracked;
		}

		public ARTrackableStateChangedEvent(bool tracked, string markername)
		{
			this.IsTracked = tracked;
			this.MarkerName = markername;
		}
	}
}