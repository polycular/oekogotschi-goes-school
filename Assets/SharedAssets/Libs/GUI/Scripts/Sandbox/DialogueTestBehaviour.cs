using SharedAssets.GuiMain;
using UnityEngine;
using System.Collections;

public class DialogueTestBehaviour : MonoBehaviour
{
    public GameObject GuiObjectManager;
    public bool Test = false;
    public bool Test2 = false;

    // Use this for initialization
    void Start()
    {
        //GuiObjectManager.GetComponent<DialogueManagerBehaviour>().getDialogue("1").show();
        GUIManagerBehaviour.Instance.afterDialogueHide += Instance_afterDialogueHide;
    }

    void Instance_afterDialogueHide(DialogueBehaviour dialogue)
    {
        //GuiObjectManager.GetComponent<DialogueManagerBehaviour>().getDialogue("2").show();
    }

    // Update is called once per frame
    void Update()
    {
        if (Test)
        {
            Test = false;
            executeTest();
        }
        if (Test2)
        {
            Test2 = false;
            executeTest();
        }
    }

    private void executeTest()
    {
        GuiObjectManager.GetComponent<DialogueManagerBehaviour>().getDialogue("1").show();
    }



    private void executeTest2()
    {
        GuiObjectManager.GetComponent<DialogueManagerBehaviour>().getDialogue("2").show();
    }
}
