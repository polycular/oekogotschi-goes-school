using System;
using SharedAssets.GuiMain;
using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    public class MenuBehaviour : MonoBehaviour
    {
        //callback signature must be: void callback()
        public Action RestartCallback;
        public Action ExitCallback;

        public void update()
        {
            GUIManagerBehaviour.Instance.progress(this);
        }
    }
}
