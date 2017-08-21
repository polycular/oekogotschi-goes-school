using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SharedAssets.GuiMain
{
    public class QuestlogControllerBehaviour : PanelController
    {
        public OnPauseHandler onQuestLogPause;
        public OnContinueHandler onQuestLogContinue;

        //fields
        public UILabel LatestQuestHeader;
        public UILabel LatestQuestContent;
        public UILabel DialogueEntries;
        public UIScrollView DialogueEntriesScrollView;
        public UIScrollBar DialogueEntriesScrollBar;
        public int MaxDialogueEntries = 15;
        public float TweenDuration = 0.5f;
        public GameObject QuestLog;

        //public GameObject DialogueEntryPrefab; //new entries must be copied, because the UIPanel must (!) be the same as the QuestPanel for rendering to work!
        //private int mDialogueEntryID;

        public bool DebugButton;
        public bool DebugButton2;
        public bool DebugButton3;
        public bool DebugButton4;

        private string mLatestQuestLabel = String.Empty;
        private string mLatestQuestText = String.Empty;
        private string mLastHeaderUsed = String.Empty;

        private int mDialogueEntries = 0;


        public void Awake()
        {
            QuestLog.GetComponent<UIPanel>().alpha = 0;
        }

        public void Start()
        {
            DialogueEntries.text = String.Empty;
        }

        public void Update()
        {
            if (DebugButton)
            {
                DebugButton = false;
                addDialogueToLog("Lorem  ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna PCE", "Meister Lee Gong", false);
            }
            if (DebugButton2)
            {
                DebugButton2 = false;
                addDialogueToLog("Et tu brute? Alea iacta est! Italia terra fecundia est. Y U speek to me?", "Meister Opi Um", false);
            }
            if (DebugButton3)
            {
                DebugButton3 = false;
                addDialogueToLog("Lorem  ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna PCE", "Meister Lee Gong", true);
            }
            if (DebugButton4)
            {
                DebugButton4 = false;
                addDialogueToLog("Et tu brute? Alea iacta est! Italia terra fecundia est. Y U speek to me?", "Meister Opi Um", true);
            }

        }

        public void hideMenuByGuiTrigger()
        {
            if (!base.getInputLocked())
            {
                base.lockInput();
                hide();
            }
                
        }

        public void showMenuGuiTrigger()
        {
            if (!base.getInputLocked())
            {
                base.lockInput();
                show();
            }
        }

        public void addDialogueToLog(string dialogueText, string header, bool quest)
        {
            if (!quest)
            {
                string headerForUse = String.Empty;
                string headerSuffix = ":  ";
                string bbColor = "[1DAAAB]";

                if (mLastHeaderUsed == String.Empty || mLastHeaderUsed != header)
                {
                    headerForUse = string.Format("{0}[b]{1}{2}[/b][-]", bbColor, header, headerSuffix);
                }
                else if (mLastHeaderUsed == header)
                {
                    headerForUse = String.Empty;
                }

                DialogueEntries.text += string.Format("{0}{1}\n\n", headerForUse, dialogueText);
                //automatically scrolls the scrollview to the latest entry
                DialogueEntriesScrollBar.value = 1f;
                DialogueEntriesScrollView.UpdatePosition();

                mLastHeaderUsed = header;
                mDialogueEntries++;

                //limit entries due to max. vertex limit in a panel controller ~ 145 dialogues
                if (mDialogueEntries > MaxDialogueEntries)
                {
                    string[] parts = DialogueEntries.text.Split(new string[] { "\n\n" },
                        StringSplitOptions.RemoveEmptyEntries);

                    StringBuilder builder = new StringBuilder();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        builder.Append(parts[i]);
                        builder.Append("\n\n");
                    }
                    mDialogueEntries--;
                    DialogueEntries.text = builder.ToString();
                    DialogueEntriesScrollBar.value = 1f;
                    DialogueEntriesScrollView.UpdatePosition();
                }
            }
            else
            {
                mLatestQuestLabel = header;
                mLatestQuestText = dialogueText;

                updateQuestlog();
            }
            //this code can be used to implement the DialogueEntries with single Labels

            //NOTE: anchoring is required due setting the position directly isnt possible - no reference point available

            //if there are no children, anchor directly to the top of the scroll view 
            //if (DialogueScrollView.transform.childCount == 0)
            //{
            //anchor to scrollview directly - yes its working with scrolling.
            //}
            //else
            //{
            //get anchor element - the last element - via GO name with mDialogueEntryID
            //}

            //go.transform.parent = DialogueScrollView.transform;
            //go.SetActive(true);
        }

        private void updateQuestlog()
        {
            LatestQuestHeader.text = mLatestQuestLabel;
            LatestQuestContent.text = mLatestQuestText;
        }

        private void show()
        {
            List<Collider> questLogColliders = QuestLog.gameObject.GetComponentsInChildren<Collider>().ToList();
            foreach (var collider in questLogColliders)
            {
                collider.isTrigger = true;
            }

            if (onQuestLogPause != null)
            {
                onQuestLogPause();
            }

            TweenAlpha.Begin(QuestLog.gameObject, TweenDuration, 1);
        }

        private void hide()
        {
            List<Collider> questLogColliders = QuestLog.gameObject.GetComponentsInChildren<Collider>().ToList();
            foreach (var collider in questLogColliders)
            {
                collider.isTrigger = false;
            }

            TweenAlpha.Begin(QuestLog.gameObject, TweenDuration, 0);
            StartCoroutine("delayHideActions", TweenDuration);
        }

        private IEnumerator delayHideActions(float delay)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);

            if (onQuestLogContinue != null)
            {
                onQuestLogContinue();
            }
        }

        public IEnumerable<object> questLogColliders { get; set; }
    }
}

