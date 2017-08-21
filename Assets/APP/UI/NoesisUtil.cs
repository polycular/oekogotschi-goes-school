using Noesis;
using UnityEngine;

namespace EcoGotchi.UI
{
	public static class NoesisUtil
	{
		public static T GetRootNode<T>() where T : class
		{
			object root = default(T);

			NoesisGUIPanel noesis = GameObject.FindObjectOfType<NoesisGUIPanel>();
			root = noesis.GetContent();

			return root as T;
		}

		public static ElementType FetchElement<ElementType>(FrameworkElement container, string xName)
		{
			var element = container.FindName(xName);
			Debug.Assert(element != null);
			return (ElementType)element;
		}

		public static T FetchResource<T>(FrameworkElement container, string xKey)
		{
			var element = container.FindResource(xKey);
			Debug.Assert(element != null);
			return (T)element;
		}

		/*
		 *	Get the corresponding width to 1920 px height dependent on the device's aspect ratio
		 *	Fixed 1920 x 1080 size is necessary in xaml for fixed pixel value layouts
		 *	(Therefore setting size = screen size in XAML isn't an option)
		 *	Yes, it's a bit hacky.
		 */
		public static int GetWidth()
		{
			float screenHeight = Screen.height;
			float screenWidth = Screen.width;

			float aspectRatio = screenWidth / screenHeight;

			// precision issue handling
			aspectRatio = Mathf.Round(aspectRatio *= 100);

			if (aspectRatio >= 54 && aspectRatio <= 58)
				return 1080;
			else if (aspectRatio >= 62 && aspectRatio <= 64)
				return 1200;
			else if (aspectRatio >= 66 && aspectRatio <= 68)
				return 1280;
			else if (aspectRatio >= 73 && aspectRatio <= 77)
				return 1440;

			return -1;
		}

		public static Visibility BoolToVis(bool value, Visibility hideAs = Visibility.Hidden, bool invert = false)
		{
			bool visible = value ^ invert;

			if (hideAs == Visibility.Hidden)
				return visible ? Visibility.Visible : Visibility.Hidden;
			else if (hideAs == Visibility.Collapsed)
				return visible ? Visibility.Visible : Visibility.Collapsed;
			else
				return BoolToVis(visible, Visibility.Hidden);
		}

		public static bool VisToBool(Visibility visibility)
		{
			if (visibility == Visibility.Visible)
				return true;
			else
				return false;
		}
	}
}