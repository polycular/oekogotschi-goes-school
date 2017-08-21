using System;
using System.IO;
using UnityEngine;

namespace Polycular.Logging
{
	/// <summary> Additionally writes a log file to Application.PersistantDataPath/logs. </summary>
	sealed class FileLog : BaseLog
	{
		StreamWriter m_streamWriter;

		public FileLog()
		{
			Init();
		}

		public FileLog(IPolycularLog innerlog)
		{
			InnerLog = innerlog;
			Init();
		}

		void Init()
		{
			var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
			var unixTime = (long)timeSpan.TotalSeconds;

			string path = Application.persistentDataPath;
			path = Path.Combine(path, "logs");
			Directory.CreateDirectory(path);
			path = Path.Combine(path, unixTime + "_log.txt");

			m_streamWriter = File.AppendText(path);
			m_streamWriter.AutoFlush = true;
		}

		public override void LogException(Exception exception, UnityEngine.Object context)
		{
			m_streamWriter.WriteLine(exception);
			base.LogException(exception, context);
		}

		public override void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			m_streamWriter.WriteLine(string.Format(format, args));
			base.LogFormat(logType, context, format, args);
		}

		public override void Close()
		{
			m_streamWriter.Close();
			base.Close();
		}
	}
}