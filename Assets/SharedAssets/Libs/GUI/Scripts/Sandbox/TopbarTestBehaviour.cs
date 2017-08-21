using UnityEngine;
using System.Collections;
using GuiMain;

public class TopbarTestBehaviour : MonoBehaviour
{
    public bool TriggerClear = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerClear)
        {
            TriggerClear = false;
            SharedAssets.GuiMain.GUIManagerBehaviour.Instance.clearTopBar();
        }
    }
}
