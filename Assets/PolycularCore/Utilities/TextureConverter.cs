using UnityEngine;

namespace Polycular.Utilities
{
	public static class TextureConverter
	{
		public static Texture2D ConvertToPremultipliedAlpha(Texture2D source)
		{
			Color[] srcColors = source.GetPixels();
			Color[] dstColors = new Color[srcColors.Length];

			for (int i = 0; i < srcColors.Length; i++)
			{
				Color srcColor = srcColors[i];
				float a = srcColor.a;
				dstColors[i] = new Color(srcColor.r * a, srcColor.g * a, srcColor.b * a, a);
			}

			Texture2D result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
			result.SetPixels(dstColors);
			result.Apply();
			return result;
		}
	}
}