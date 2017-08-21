using UnityEngine;
using System.Collections;

using SharedAssets.GuiMain;
namespace SharedAssets.MetaGameScene
{
    public class DisableGUIBehaviour : MonoBehaviour
    {

        void OnEnable()
        {
            if (GUIManagerBehaviour.Instance != null)
            {
                GUIManagerBehaviour.Instance.Helper.Root.GetComponent<UIPanel>().alpha = 0f;
            }
        }

        void OnDisable()
        {
            if (GUIManagerBehaviour.Instance != null)
            {
                GUIManagerBehaviour.Instance.Helper.Root.GetComponent<UIPanel>().alpha = 1f;
            }
        }
    }
}