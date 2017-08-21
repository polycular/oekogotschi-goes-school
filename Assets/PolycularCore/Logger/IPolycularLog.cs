using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Polycular.Logging
{
	interface IPolycularLog : ILogHandler
	{
		IPolycularLog InnerLog { get; }
		void Close();
	}
}

