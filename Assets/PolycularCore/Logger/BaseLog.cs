using System;
using UnityEngine;

namespace Polycular.Logging
{
	class BaseLog : IPolycularLog
	{
		public IPolycularLog InnerLog { get; protected set; }

		public virtual void LogException(Exception exception, UnityEngine.Object context)
		{
			if (InnerLog != null)
				InnerLog.LogException(exception, context);
			else
				PolycularLog.DefautLogger.LogException(exception, context);
		}

		public virtual void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			if (InnerLog != null)
				InnerLog.LogFormat(logType, context, format, args);
			else
				PolycularLog.DefautLogger.LogFormat(logType, context, format, args);
		}
		public virtual void Close()
		{
			if (InnerLog != null)
				InnerLog.Close();
		}
	}
}
