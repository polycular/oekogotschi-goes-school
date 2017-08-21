using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SharedAssets.GuiMain
{
    public class MenuControllerBehaviour : PanelController
    {
        public event OnPauseHandler onMenuPause;
        public event OnContinueHandler onMenuContinue;
        public event OnGuiMessagesOccuredHandler onMenuGuiMessagesOccured;

        public bool ShowDebug;
        public float TweenDuration = 0.5f;
        public GameObject MenuPanel;

        public DialoguePanelControllerBehaviour DialoguePanelController;

        private Action mRestartCallback;
        private Action mExitCallback;
        //private Action mOptionsCallback;

        private List<GUIMessage> mGuiMessages = new List<GUIMessage>();

        public void Awake()
        {
            MenuPanel.GetComponent<UIPanel>().alpha = 0;
        }


        //set by gui manager <- methods send by MenuBehaviour <- done by user
        public void setRestartCallback(Action restartCallback)
        {
            if (restartCallback != null)
                this.mRestartCallback = restartCallback;
        }

        public void setExitCallback(Action exitCallback)
        {
            if (exitCallback != null)
                this.mExitCallback = exitCallback;
        }

        //public void setOptionsCallback(Action optionsCallback)
        //{
        //    if (optionsCallback != null)
        //        this.mOptionsCallback = optionsCallback;
        //}

        #region GUI-invoked methods

        public void restartByGuiTrigger()
        {
            //hide all dialgoue windows and suppress their event triggers
            DialoguePanelController.hideWithoutTweenAndEvent();

            if (!base.getInputLocked())
            {
                if (ShowDebug)
                    Debug.Log("RESTART");

                base.lockInput();

                restart(mRestartCallback);
                hide();
            }
        }

        public void exitByGuiTrigger()
        {
            //hide all dialgoue windows and suppress their event triggers
            DialoguePanelController.hideWithoutTweenAndEvent();

            if (!base.getInputLocked())
            {
                if (ShowDebug)
                    Debug.Log("EXIT");

                base.lockInput();
                exit(mExitCallback);
                hide();
            }
        }

        //public void optionsByGuiTrigger()
        //{
        //    if (!base.getInputLocked())
        //    {
        //        if (ShowDebug)
        //            Debug.Log("OPTIONS");

        //        base.lockInput();
        //        options(mOptionsCallback);
        //        hide();
        //    }
        //}

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

        #endregion

        public void updateMenu(MenuBehaviour menu)
        {
            if (menu.RestartCallback != null)
                this.mRestartCallback = menu.RestartCallback;

            if (menu.ExitCallback != null)
                this.mExitCallback = menu.ExitCallback;

            //if (menu.OptionsCallback != null)
            //    this.mOptionsCallback = menu.OptionsCallback;
        }

        private void show()
        {
            List<Collider> menuColliders = MenuPanel.gameObject.GetComponentsInChildren<Collider>().ToList();
            foreach (var collider in menuColliders)
            {
                collider.isTrigger = true;
            }

            //trigger the PanelController to fire the pause event
            if (onMenuPause != null)
            {
                onMenuPause();
            }

            TweenAlpha.Begin(MenuPanel.gameObject, TweenDuration, 1);
        }

        private void hide()
        {
            List<Collider> menuColliders = MenuPanel.gameObject.GetComponentsInChildren<Collider>().ToList();
            foreach (var collider in menuColliders)
            {
                collider.isTrigger = false;
            }

            TweenAlpha.Begin(MenuPanel.gameObject, TweenDuration, 0);
            StartCoroutine("delayHideActions", TweenDuration);
        }

        private IEnumerator delayHideActions(float delay)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);

            if (onMenuContinue != null)
            {
                onMenuContinue();
            }
        }

        private void restart(Action restartCallback)
        {
            if (restartCallback != null)
            {
                restartCallback.Invoke();
            }
            else
            {
                mGuiMessages.Add(new GUIMessage("MenuBehaviour",GUIMessage.Type.WARNING, "No Callback is registered for the 'Restart' button"));
                if (onMenuGuiMessagesOccured != null)
                {
                    onMenuGuiMessagesOccured(mGuiMessages);
                    mGuiMessages.Clear();
                }
            }
        }

        private void exit(Action exitCallback)
        {
            if (exitCallback != null)
            {
                exitCallback.Invoke();
            }
            else
            {
                mGuiMessages.Add(new GUIMessage("MenuBehaviour", GUIMessage.Type.WARNING, "No Callback is registered for the 'Exit' button"));
                if (onMenuGuiMessagesOccured != null)
                {
                    onMenuGuiMessagesOccured(mGuiMessages);
                    mGuiMessages.Clear();
                }
            }
        }

        //private void options(Action optionCallback)
        //{
        //    if (optionCallback != null)
        //    {
        //        mOptionsCallback.Invoke();
        //    }
        //    else
        //    {
        //        mGuiMessages.Add(new GUIMessage("MenuBehaviour", GUIMessage.Type.WARNING, "No Callback is registered for the 'Options' button"));
        //        if (onMenuGuiMessagesOccured != null)
        //        {
        //            onMenuGuiMessagesOccured(mGuiMessages);
        //            mGuiMessages.Clear();
        //        }
        //    }
        //}
    }
}
