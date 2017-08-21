namespace Polycular.Utilities
{
	public static class StringExtensions
	{
		public static string ToCamelCase(this string str)
		{
			return char.ToLower(str[0]).ToString() + str.Substring(1);
		}
	}
}
