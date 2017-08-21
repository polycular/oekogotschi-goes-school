namespace Http
{
	public class QueuedRequest
	{
		public enum NetworkDirection
		{
			INCOMING,
			OUTGOING
		}

		public Request Request { get; private set; }
		public bool IsOutdated { get; set; }
		public NetworkDirection Direction  { get; private set; }

		public QueuedRequest(Request req)
		{
			Request = req;

			if (req.Method == Verbs.HEAD || Request.Method == Verbs.GET || Request.Method == Verbs.CONNECT)
				Direction = NetworkDirection.INCOMING;
			else
			{
				Direction = NetworkDirection.OUTGOING;
			}
		}
	}
}