using Noesis;
using Polycular.Utilities;
using UnityEngine;

namespace EcoGotchi.UI
{
	public static class ImageBrushUtil
	{
		public static ImageBrush GetImgBrushFromTex(Texture2D tex)
		{
			if (tex == null)
				return null;

			var multipliedAlphaTex = TextureConverter.ConvertToPremultipliedAlpha(tex);

			var imgBrush = new ImageBrush()
			{
				ImageSource = new TextureSource(multipliedAlphaTex),
				Stretch = Stretch.Uniform
			};

			return imgBrush;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="relPath">relative path with the "Assets" folder as root and NO "/" at the beginning</param>
		/// <returns></returns>
		public static ImageBrush GetImgBrushFromTex(string relPath)
		{
			string path = StreamingAssetsUtil.GetPlatformPath(relPath);

			var www = new WWW(path);
			while (!www.isDone) ;
			var tex = www.texture;

			return GetImgBrushFromTex(tex);
		}
	}
}