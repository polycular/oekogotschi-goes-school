using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SharedAssets.GuiMain
{
    public class PanelController : MonoBehaviour
    {
        public delegate void OnGuiMessagesOccuredHandler(List<GUIMessage> messages);
        public delegate void AfterHideTweenFinishedHandler(DialogueBehaviour dialogue);
        public delegate void BeforeShowTweenStartedHandler(DialogueBehaviour dialogue);
        public delegate void OnDialogueProgressedHandler(DialogueBehaviour dialogue);
        public delegate void OnDialogueDecisionHandler(DialogueBehaviour dialogue, bool decision);

        public delegate void OnPauseHandler();
        public delegate void OnContinueHandler();

        private bool mInputLocked;
        private float mInputLockDuration = 0.7f;

        public bool getInputLocked()
        {
            return mInputLocked;
        }

        public void lockInput()
        {
            if (!mInputLocked)
            {
                mInputLocked = true;
                StartCoroutine("unlock");
            }
        }

        private IEnumerator unlock()
        {
            yield return new WaitForSeconds(mInputLockDuration);
            mInputLocked = false;

        }
    }
}
