using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polycular.Logging
{
	static class PolycularLog
	{
		public static ILogHandler DefautLogger { get; private set; }

		public static void AttachToUnity(IPolycularLog logger)
		{
			if (DefautLogger != null)
				return;

			DefautLogger = Debug.logger.logHandler;
			Debug.logger.logHandler = logger;
		}

		public static void DetachFromUnity()
		{
			Debug.logger.logHandler = DefautLogger;
		}
	}
}

