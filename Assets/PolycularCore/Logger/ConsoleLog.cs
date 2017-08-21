using System;
using UnityEngine;

namespace Polycular.Logging
{
	class LogConstraints
	{
		bool[] m_logTypes = new bool[5];
		public void EnableLogTypes(params LogType[] types)
		{
			foreach (int type in types)
				m_logTypes[type] = true;
		}

		public void DisableLogTypes(params LogType[] types)
		{
			foreach (int type in types)
				m_logTypes[type] = false;
		}

		public bool ShouldLog(LogType logType)
		{
			return m_logTypes[(int)logType];
		}
	}

	/// <summary> Filters logs based on LogType: Log, Warning, Error, Assert, Exception. </summary>
	sealed class ConsoleLog : BaseLog
	{
		public LogConstraints LogConstraint { get; set; }

		public ConsoleLog()
		{
			LogConstraint = new LogConstraints();
		}

		public ConsoleLog(IPolycularLog innerlog)
		{
			LogConstraint = new LogConstraints();
			InnerLog = innerlog;
		}

		public override void LogException(Exception exception, UnityEngine.Object context)
		{
			if (!LogConstraint.ShouldLog(LogType.Exception))
				return;

			base.LogException(exception, context);
		}

		public override void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			if (!LogConstraint.ShouldLog(logType))
				return;

			base.LogFormat(logType, context, format, args);
		}

		public override void Close()
		{
			base.Close();
		}
	}
}
