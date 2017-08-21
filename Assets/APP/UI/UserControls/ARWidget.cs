using Noesis;
using Polycular;
using UnityEngine;


namespace EcoGotchi.UI
{
	[UserControlSource("Assets/APP/UI/UserControls/ARWidget.xaml")]
	public class ARWidget : UserControl
	{
		Image _imgCanvas;


		public ARWidget()
		{ }

		public void OnPostInit()
		{
			_imgCanvas = FindName("_imgCanvas") as Image;

			Eventbus.Instance.AddListener<ARTexReadyEvent>((ev) =>
			{
				var tev = ev as ARTexReadyEvent;
				SetArFeed(tev.ArTex);
			}, this);
		}

		void SetArFeed(Texture arTex)
		{
			if (arTex != null)
			{
				_imgCanvas.Source = new TextureSource(arTex);
			}
			else
			{
				Debug.LogWarning("ArFeed texture is null");
			}
		}
	}
}