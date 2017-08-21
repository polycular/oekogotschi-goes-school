using UnityEngine;
using Vuforia;

namespace EcoGotchi
{
	public class SetFocusMode : MonoBehaviour
	{
		public CameraDevice.FocusMode FocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;

		void Start()
		{
			FindObjectOfType<VuforiaBehaviour>().RegisterVuforiaStartedCallback(() => {
				CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
			});
		}
	}
}
