using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polycular.Logging
{
	/// <summary> Filters logs based on their StackTrace and a set of filter words. </summary>
	sealed class TraceFilterLog : BaseLog
	{
		List<string> m_filter = new List<string>();

		public TraceFilterLog()
		{
		}

		public TraceFilterLog(IPolycularLog innerlog)
		{
			InnerLog = innerlog;
		}

		public void AddFilter(string filter)
		{
			m_filter.Add(filter);
		}

		public override void LogException(Exception exception, UnityEngine.Object context)
		{
			if (IsFiltered())
				return;

			base.LogException(exception, context);
		}

		public override void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			if (IsFiltered())
				return;

			base.LogFormat(logType, context, format, args);
		}

		bool IsFiltered()
		{
			var trace = Environment.StackTrace; // Returns whole stack trace as string.

			foreach (var filter in m_filter)
				if (trace.Contains(filter))
					return true;

			return false;
		}

		public override void Close()
		{
			base.Close();
		}
	}
}
