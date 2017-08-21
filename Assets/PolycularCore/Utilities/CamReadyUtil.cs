using UnityEngine;

namespace Polycular.Utilities
{
	public class CamReadyUtil : MonoBehaviour
	{
		public Camera ArRenderCam;

		Texture2D m_tex2d;
		RenderTexture m_rtex;

		void Start()
		{
			float aspect = (Screen.width < Screen.height) ? ((float)Screen.height / Screen.width) : ((float)Screen.width / Screen.height);
			int rtWidth = Screen.width;
			int rtHeight = Screen.height;

			if (aspect != (16f / 9f) && aspect != (16f / 10f))
			{
				rtWidth = 1080;
				rtHeight = 1920;
			}

			m_rtex = new RenderTexture(rtWidth, rtHeight, 1);
			RenderTexture.active = m_rtex;
			ArRenderCam.targetTexture = m_rtex;

			m_tex2d = new Texture2D(rtWidth, rtHeight);
		}

		void OnPostRender()
		{
			m_tex2d.ReadPixels(new Rect(0, 0, m_rtex.width, m_rtex.height), 0, 0);

			int width = Screen.width;
			int height = Screen.height;

			int widthQuarter = width / 4;
			int heightQuarter = height / 4;

			Color sampleColor1 = m_tex2d.GetPixel(3 * widthQuarter, 3 * heightQuarter);
			Color sampleColor2 = m_tex2d.GetPixel(widthQuarter, 3 * heightQuarter);
			Color sampleColor3 = m_tex2d.GetPixel(widthQuarter, heightQuarter);
			Color sampleColor4 = m_tex2d.GetPixel(3 * widthQuarter, heightQuarter);

			Color sampleSum = sampleColor1 + sampleColor2 + sampleColor3 + sampleColor4;

			bool isBlack = sampleSum.r < 0.1f && sampleSum.g < 0.1f && sampleSum.b < 0.1f;
			if (!isBlack)
			{
				ArRenderCam.targetTexture = null;
				RenderTexture.active = null;
				m_tex2d = null;

				Eventbus.Instance.FireEvent<CamReadyEvent>(new CamReadyEvent());
				this.enabled = false;
			}
		}
	}
}