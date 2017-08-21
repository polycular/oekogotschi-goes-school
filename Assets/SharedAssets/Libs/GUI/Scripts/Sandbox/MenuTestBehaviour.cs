using SharedAssets.GuiMain;
using UnityEngine;
using System.Collections;

public class MenuTestBehaviour : MonoBehaviour
{
    public MenuBehaviour MenuBehaviour;
    public bool DebugUpdate;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugUpdate)
        {
            DebugUpdate = false;
            initMenu();
        }
    }

    private void initMenu()
    {
        MenuBehaviour.RestartCallback = restartMyGame;
        MenuBehaviour.update();
    }

    private void restartMyGame()
    {
        Debug.Log("RRRRRRRRRRRRRRRESTART - callback is working");
    }
}
