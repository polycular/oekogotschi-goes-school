using UnityEngine;
using System.Collections;

using SharedAssets.GuiMain;
using SharedAssets.GameManagement;

namespace SharedAssets.MetaGameScene
{
    /// <summary>
    /// Each Trial has a Station in the MetaGame, form which it('s Session) can be started
    /// 
    /// Connects an Image target to the Interactive process of starting the Trial (MiniGame)
    /// </summary>
    public class TrialStationBehaviour : MonoBehaviour
    {
        [Tooltip("The Name of the Trial/Minigame (Prefab)")]
        public string TrialName = null;
        public DialogueBehaviour RequestDialogue = null;
        public float WaitSecondsBeforeReRequest = 5f;

        /// <summary>
        /// Starts the Interaction process with the user, aksing him if he wants to start the Trial
        /// Does nothing if the last request has not cooled down
        /// </summary>
        /// <returns></returns>
        public void requestTrial()
        {
            if (!mIsRequesting && RequestDialogue != null)
            {
                mIsRequesting = true;
                RequestDialogue.show();
            }
        }

        //use this to start a trial directly from code
        public MiniGameSessionBehaviour startTrialCold()
        {
            mTrial = MetaGame.Instance.Behaviour.MiniGames[TrialName];
            return startTrial();
        }

        public MiniGameSessionBehaviour startTrial()
        {
            if (mTrial != null)
                return mTrial.startSession();
            else
                return null;
        }

        
        protected MiniGameBehaviour mTrial = null;
        protected float mLastRequestCompletedTimestamp = 0f;
        protected bool mIsRequesting = false;


        void onDialogueDecision(DialogueBehaviour dialogue, bool decision)
        {
            if (dialogue == RequestDialogue && decision)
            {
                dialogue.hide(true);
                mTrial.startSession();
            }
        }
        void afterDialogueHide(DialogueBehaviour dialogue)
        {
            if (dialogue == RequestDialogue)
            {
                mIsRequesting = false;
                mLastRequestCompletedTimestamp = Time.timeSinceLevelLoad;
            }
        }

        // Use this for initialization
        void Start()
        {
            mTrial = MetaGame.Instance.Behaviour.MiniGames[TrialName];
            if (mTrial == null)
                ToolbAR.LogAR.logWarning("Could connect to Trial/MiniGame, none found with Name \"" + TrialName + "\"", this, this);

            GUIManagerBehaviour.Instance.onDialogueDecision -= onDialogueDecision;
            GUIManagerBehaviour.Instance.onDialogueDecision += onDialogueDecision;
            GUIManagerBehaviour.Instance.afterDialogueHide -= afterDialogueHide;
            GUIManagerBehaviour.Instance.afterDialogueHide += afterDialogueHide;
        }

        // Update is called once per frame
        void Update()
        {
            if (!mIsRequesting && Time.timeSinceLevelLoad > (mLastRequestCompletedTimestamp + WaitSecondsBeforeReRequest))
            {
                requestTrial();
            }
        }

        void OnEnable()
        {
            requestTrial();
        }
        void OnDisable()
        {
            RequestDialogue.hide();
            mIsRequesting = false;
        }
    }
}