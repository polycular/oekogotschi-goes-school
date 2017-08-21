using System;
using System.Security;

namespace Polycular.Utilities
{
	public static class SecureStringExtensions
	{
		public static string ConvertToUnsecureString(this SecureString sstr)
		{
			if (sstr == null)
				throw new ArgumentNullException();

			IntPtr unmanagedStr = IntPtr.Zero;
			try
			{
				unmanagedStr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(sstr);
				return System.Runtime.InteropServices.Marshal.PtrToStringUni(unmanagedStr);
			}
			finally
			{
				System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(unmanagedStr);
			}
		}
	}
}
