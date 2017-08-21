using UnityEngine;
using System.Collections;
using SharedAssets.GuiMain;

public class HintTestBehaviour : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GUIManagerBehaviour.Instance.Hints.setCombinedHint("Test Text for IA Sprites", InteractionSpriteListBehaviour.InteractionType.FIND_LOST_MARKER, true, true);
        GUIManagerBehaviour.Instance.Hints.setCombinedHintAsWarning();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
