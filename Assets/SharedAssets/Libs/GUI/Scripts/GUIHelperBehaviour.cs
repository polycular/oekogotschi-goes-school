using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    public class GUIHelperBehaviour : MonoBehaviour
    {

        public GUIManagerBehaviour GUIManager = null;
        public UIRoot Root = null;

        public float Width
        {
            get
            {
                float ratio = (float)Root.activeHeight / Screen.height;
                return Mathf.Ceil(Screen.width * ratio);
            }
        }

        public float Height
        {
            get
            {
                float ratio = (float)Root.activeHeight / Screen.height;
                return Mathf.Ceil(Screen.height * ratio);
            }
        }

        public Vector3 convertScreenSpaceToGUISpace(Vector3 screenPoint)
        {
            //Convert to [-1f..1f] range
            screenPoint.x = ((screenPoint.x / Screen.width) - 0.5f) * 2f;
            screenPoint.y = ((screenPoint.y / Screen.height) - 0.5f) * 2f;

            float ratio = (float)Root.activeHeight / Screen.height;

            float width = Mathf.Ceil(Screen.width * ratio);
            float height = Mathf.Ceil(Screen.height * ratio);

            return new Vector3(
                screenPoint.x * width * 0.5f,
                screenPoint.y * height * 0.5f,
                0f);
        }
    }
}
