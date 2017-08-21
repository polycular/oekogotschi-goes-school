using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GuiMain.GuiInteraction;

namespace SharedAssets.GuiMain
{
    public class GUIManagerBehaviour : MonoBehaviour
    {
        public event PanelController.BeforeShowTweenStartedHandler beforeDialogueShow;
        public event PanelController.AfterHideTweenFinishedHandler afterDialogueHide;
        public event PanelController.OnDialogueProgressedHandler onDialogueProgressed;
        public event PanelController.OnDialogueDecisionHandler onDialogueDecision;
        public event PanelController.OnPauseHandler onGuiPause;
        public event PanelController.OnContinueHandler onGuiContinue;

        public GUIHelperBehaviour Helper = null;
        public HintManagerBehaviour Hints = null;
        public LiveScoreBehaviour LiveScores = null;
        public List<PanelController> PanelList = new List<PanelController>();

        public GameObject ProfileButton;

        private static GUIManagerBehaviour mInstance;
        public static GUIManagerBehaviour Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<GUIManagerBehaviour>();
                }
                return mInstance;
            }
        }

        private static InputManagerBehaviour mInputManager;
        public static InputManagerBehaviour InputManagerInstance
        {
            get
            {
                if (mInputManager == null)
                {
                    mInputManager = FindObjectOfType<InputManagerBehaviour>();
                }
                return mInputManager;
            }
        }



        //debug
        public bool DebugButton;
        public bool ShowDebug = false;

        public void show(MonoBehaviour values, bool fading = true)
        {
            resolve(values, mResolveType.Show, fading);
        }

        public void hide(MonoBehaviour values, bool fading = true)
        {
            resolve(values, mResolveType.Hide, fading);
        }

        public void progress(MonoBehaviour values)
        {
            resolve(values, mResolveType.Progress);
        }

        public void hideAllDialogues(bool fading = false)
        {
            if (PanelList == null || PanelList.Count == 0)
                return;

            var dpc = (DialoguePanelControllerBehaviour)PanelList.FirstOrDefault(x => x.GetType() == typeof(DialoguePanelControllerBehaviour));

            if (dpc == null)
                return;

            dpc.hideAll(fading);
        }

        public void clearTopBar()
        {
            if (PanelList == null || PanelList.Count == 0)
                return;

            var tpc = (TopPanelControllerBehaviour)PanelList.FirstOrDefault(x => x.GetType() == typeof(TopPanelControllerBehaviour));

            if (tpc == null)
                return;

            tpc.clearTopBar();
        }

        public void setStateProfileButton(bool enabled)
        {
            ProfileButton.SetActive(enabled);
        }

        private enum mResolveType
        {
            Show,
            Hide,
            Progress
        }

        #region UnityMethods

        private void Awake()
        {
            AddEventListeners();
        }

        private void Update()
        {
            //debug button
            if (DebugButton)
            {
                DebugButton = false;
            }
        }

        #endregion

        private void AddEventListeners()
        {
			if (PanelList == null)
				return;

            if (PanelList.Count != 0)
            {
                foreach (PanelController pc in PanelList)
                {
					if (pc == null)
						continue;

                    if (pc.GetType() == typeof(DialoguePanelControllerBehaviour))
                    {
                        ((DialoguePanelControllerBehaviour)pc).afterHideTweenFinished +=
                            notifyClients_afterTweenFinished;
                        ((DialoguePanelControllerBehaviour)pc).onDialogueProgressed +=
                            notifyClients_onDialogueProgressed;
                        ((DialoguePanelControllerBehaviour)pc).onDialogueGuiMessagesOccured +=
                            showGuiMessages;
                        ((DialoguePanelControllerBehaviour)pc).onDialogueDecision +=
                            notifyClients_onDialogueDecision;
                        ((DialoguePanelControllerBehaviour)pc).beforeShowTweenStarted +=
                            notifyClients_beforeDialogueShow;
                    }
                    if (pc.GetType() == typeof(TopPanelControllerBehaviour))
                    {
                        //NOTE: disabled because of frame based update calls!
                        //((TopPanelControllerBehaviour) pc).onTopBarGuiMessagesOccured += showGuiMessages;
                    }
                    if (pc.GetType() == typeof(MenuControllerBehaviour))
                    {
                        ((MenuControllerBehaviour)pc).onMenuPause += notifyClients_onGuiPause;
                        ((MenuControllerBehaviour)pc).onMenuContinue += notifyClients_onGuiContinue;
                        ((MenuControllerBehaviour)pc).onMenuGuiMessagesOccured += showGuiMessages;
                    }
                    if (pc.GetType() == typeof(QuestlogControllerBehaviour))
                    {
                        ((QuestlogControllerBehaviour)pc).onQuestLogPause += notifyClients_onGuiPause;
                        ((QuestlogControllerBehaviour)pc).onQuestLogContinue += notifyClients_onGuiContinue;
                    }
                }
            }
        }

        #region Notify Client Events

        //forward event received from DialoguePanelController
        private void notifyClients_afterTweenFinished(DialogueBehaviour dialogue)
        {
            //Debug Output
            if (ShowDebug)
            {
                Debug.Log("--- event: AFTER tween finished");
            }

            if (afterDialogueHide != null)
            {
                afterDialogueHide(dialogue);
            }
        }

        private void notifyClients_onDialogueProgressed(DialogueBehaviour dialogue)
        {
            //Debug Output
            if (ShowDebug)
            {
                Debug.Log("--- event: dialogue progressed");
            }

            if (onDialogueProgressed != null)
            {
                onDialogueProgressed(dialogue);
            }
        }

        private void notifyClients_onDialogueDecision(DialogueBehaviour dialogue, bool decision)
        {
            //Debug Output
            if (ShowDebug)
            {
                Debug.Log("--- event: dialogue decision " + decision.ToString().ToUpper());
            }

            if (onDialogueDecision != null)
            {
                onDialogueDecision(dialogue, decision);
            }
        }

        private void notifyClients_beforeDialogueShow(DialogueBehaviour dialogue)
        {
            //Debug Output
            if (ShowDebug)
            {
                Debug.Log("--- event: BEFORE dialogue show");
            }

            if (beforeDialogueShow != null)
            {
                beforeDialogueShow(dialogue);
            }
        }

        private void notifyClients_onGuiPause()
        {
            if (ShowDebug)
            {
                Debug.Log("--- event: ON gui pause");
            }

            if (onGuiPause != null)
            {
                onGuiPause();
            }
        }

        private void notifyClients_onGuiContinue()
        {
            if (ShowDebug)
            {
                Debug.Log("--- event: ON gui continue");
            }

            if (onGuiContinue != null)
            {
                onGuiContinue();
            }
        }

        #endregion

        private void resolve(MonoBehaviour values, mResolveType type, bool fading = true)
        {
            if (PanelList == null || PanelList.Count == 0)
                return;

            //Dialogue
            //TODO: panelcontroller can register at gui manager, no manual linking in code needed!
            if (values.GetType() == typeof(DialogueBehaviour))
            {
                var dpc =
                    (DialoguePanelControllerBehaviour)
                        PanelList.FirstOrDefault(x => x.GetType() == typeof(DialoguePanelControllerBehaviour));

                if (dpc == null)
                    return;

                switch (type)
                {
                    case mResolveType.Show:
                        dpc.setDialogue((DialogueBehaviour)values, fading);
                        break;
                    case mResolveType.Hide:
                        dpc.hide(fading);
                        break;
                    case mResolveType.Progress:
                        dpc.progress((DialogueBehaviour)values);
                        break;
                }
            }

            //TopBar
            if (values.GetType() == typeof(TopBarBehaviour))
            {
                var tpc =
                    (TopPanelControllerBehaviour)
                        PanelList.FirstOrDefault(x => x.GetType() == typeof(TopPanelControllerBehaviour));

                if (tpc == null)
                    return;

                switch (type)
                {
                    case mResolveType.Show:
                        tpc.updateTopBar((TopBarBehaviour)values);
                        break;
                }
            }

            //Menu
            if (values.GetType() == typeof(MenuBehaviour))
            {
                var mc =
                    (MenuControllerBehaviour)PanelList.FirstOrDefault(x => x.GetType() == typeof(MenuControllerBehaviour));

                if (mc == null)
                    return;

                switch (type)
                {
                    case mResolveType.Progress:
                        mc.updateMenu((MenuBehaviour)values);
                        break;
                }
            }
        }

        private void showGuiMessages(List<GUIMessage> messages)
        {
            foreach (GUIMessage entry in messages)
            {
                entry.showMessage();
            }
        }
    }
}
