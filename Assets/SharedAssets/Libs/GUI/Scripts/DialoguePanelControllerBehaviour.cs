using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedAssets.GuiMain
{
	public class DialoguePanelControllerBehaviour : PanelController
    {
        #region Member
        public event AfterHideTweenFinishedHandler afterHideTweenFinished;
        public event BeforeShowTweenStartedHandler beforeShowTweenStarted;
        public event OnDialogueProgressedHandler onDialogueProgressed;
        public event OnDialogueDecisionHandler onDialogueDecision;

        public event OnGuiMessagesOccuredHandler onDialogueGuiMessagesOccured;

        //for dialogue types: 'Information', 'Blocking', 'Persistant'
        public UILabel HeaderLabelMain;
        public UILabel ContentLabelMain;
        public UISprite PortraitSpriteMain;

        public GameObject DialogueMain;
        public GameObject DialogueDecision;
        //for blocking and decision dialogues
        public GameObject BlockingTexture;

        //for 'Decision' dialogue type
        public UILabel HeaderLabelDecision;
        public UILabel ContentLabelDecision;
        public UISprite PortraitSpriteDecision;

        //for centering dialogues
        public Transform GuiMainTransform;
        public Transform ContentTransform;
        public Transform BackgroundTransform;

        public GameObject DialogueLogo;
        public string DialogueSpriteForward;
        public string DialogueSpriteLocked;

        public QuestlogControllerBehaviour QuestlogController;

        //new values for dynamic margin
        public float AdditionalBottomBlockMarginInPercent = 0;
        public float AdditionalBottomDecisionMarginInPercent = 0;
        public int ContentButtonPadding = 0;

        //save original values before applying dynamic anchor settings
        //for centering
        private float? mOrigContentBottomMargin = null;

        //default values
        public string DefaultHeader = "Meister Lee Quang";
        public Sprite DefaultSprite;

        //tweeking values
        public float TweenDuration = 0.5f;
        public UITweener TweenDecision;
        public UITweener TweenMain;
		
        public bool TypewriterEnabled = false;
        public int TypewriterCharPerSec = 0;
        public float TypewriterCharFadeIn = 0f;
        private TypewriterEffect mTypewriterEffect = null;

        //internal
        private DialogueBehaviour mCurrentDialogue;
        private int mSubDialogueCounter = 0;
        private bool mIsLocked = false;
        private bool mIsProgressed = false;

        //tween states
        private bool mMainFinalizeHappend = true;
        private bool mDecisionFinalizeHappend = true;

        private bool mMainHidden = true;
        private bool mDecisionHidden = true;

        #endregion

        #region UnityMethods

        private void Awake()
        {
            BlockingTexture.GetComponent<UITexture>().alpha = 0;

            DialogueMain.GetComponent<UIPanel>().alpha = 0;
            DialogueDecision.GetComponent<UIPanel>().alpha = 0;

            NGUITools.SetActive(DialogueMain, false);
            NGUITools.SetActive(DialogueDecision, false);

            if (DefaultHeader != null)
                HeaderLabelMain.text = DefaultHeader;
        }

        #endregion

        public void setDialogue(DialogueBehaviour dialogue, bool fading = true)
        {
            StopCoroutine("fadingHide");

            if (mIsLocked)
                return;

            mCurrentDialogue = dialogue;
            mSubDialogueCounter = 0;

            switch (mCurrentDialogue.DialogueType)
            {
                case DialogueBehaviour.Type.Information:

                    if (mCurrentDialogue.FadingTime != 0.0f)
                    {
                        StartCoroutine("fadingHide", mCurrentDialogue.FadingTime);
                    }

                    break;

                case DialogueBehaviour.Type.Block:
                case DialogueBehaviour.Type.Persistant:
                    //mIsLocked = true;
                    break;

                case DialogueBehaviour.Type.Decision:
                    mIsLocked = true;
                    break;
            }

            updateDialogue();
            show(fading);

        }

        private void updateDialogue()
        {
            List<GUIMessage> messages = new List<GUIMessage>();

            mIsProgressed = true;

            if (HeaderLabelMain != null)
            {
                if (mCurrentDialogue.Header != null)
                {
                    int headerMaxIndex = (mCurrentDialogue.Header.Count - 1);
                    if (mSubDialogueCounter <= headerMaxIndex)
                    {
                        mIsProgressed = false;

                        if (string.IsNullOrEmpty(mCurrentDialogue.Header[mSubDialogueCounter]))
                        {
                            HeaderLabelMain.text = DefaultHeader;
                            messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Header' on index {0} is empty/null - default header used", mSubDialogueCounter)));
                        }
                        else
                        {
                            HeaderLabelMain.text = mCurrentDialogue.Header[mSubDialogueCounter];
                        }
                    }
                }
                else
                {
                    HeaderLabelMain.text = DefaultHeader;
                    messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Header' is null - default header used", mSubDialogueCounter)));
                }
            }

            if (ContentLabelMain != null)
            {
                if (mCurrentDialogue.Content != null)
                {
                    int contentMaxIndex = (mCurrentDialogue.Content.Count - 1);
                    if (mSubDialogueCounter <= contentMaxIndex)
                    {
                        mIsProgressed = false;

                        if (string.IsNullOrEmpty(mCurrentDialogue.Content[mSubDialogueCounter]))
                        {
                            messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Content' on index {0} is empty/null - field is mandatory!", mSubDialogueCounter)));
                        }
                        else
                        {
                            ContentLabelMain.text = mCurrentDialogue.Content[mSubDialogueCounter];
                        }
                    }
                }
                else
                {
                    messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Content' is null - field is mandatory!", mSubDialogueCounter)));
                }
            }

            //if (PortraitSpriteMain != null)
            //{
            //    if (mCurrentDialogue.Portrait != null)
            //    {
            //        int portraitMaxIndex = (mCurrentDialogue.Portrait.Count - 1);
            //        if (mSubDialogueCounter <= portraitMaxIndex)
            //        {
            //            mIsProgressed = false;

            //            if (mCurrentDialogue.Portrait[mSubDialogueCounter] == null)
            //            {
            //                PortraitSpriteMain.spriteName = DefaultSprite.name;
            //                messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Portrait' on index {0} is empty/null - default portrait used", mSubDialogueCounter)));
            //            }
            //            else
            //            {
            //                PortraitSpriteMain.spriteName = mCurrentDialogue.Portrait[mSubDialogueCounter].name;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        PortraitSpriteMain.spriteName = DefaultSprite.name;
            //        messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue field 'Portrait' is null - default portrait used", mSubDialogueCounter)));
            //    }
            //}

            mSubDialogueCounter++;

            if (!mIsProgressed)
            {
                if (TypewriterEnabled && mTypewriterEffect != null)
                {
                    updateTypewriter();
                }
				/*
                //add processed dialogue to log if current dialogue is allowed for quests (type is blocking)
                if (mCurrentDialogue.DialogueType != DialogueBehaviour.Type.Decision)
                {
                    //quest dialogues must be of type 'Block'
                    if (mCurrentDialogue.DialogueType == DialogueBehaviour.Type.Block && mCurrentDialogue.IsQuest)
                    {
                        //add dialogue as current quest and add him to the all-dialoque section
                        QuestlogController.addDialogueToLog(ContentLabelMain.text, HeaderLabelMain.text, true);
                        QuestlogController.addDialogueToLog(ContentLabelMain.text, HeaderLabelMain.text, false);
                    }
                    else
                    {
                        QuestlogController.addDialogueToLog(ContentLabelMain.text, HeaderLabelMain.text, false);
                    }
                }
				*/
            }

            if (messages.Count > 0)
            {
                //notify gui that messages occured
                if (onDialogueGuiMessagesOccured != null)
                {
                    onDialogueGuiMessagesOccured(messages);
                }
            }
        }

        public void updateTypewriter()
        {
            if (TypewriterEnabled)
            {
                var twe = ContentTransform.GetComponent<TypewriterEffect>();
                if (twe != null)
                    Destroy(twe);

                mTypewriterEffect = ContentTransform.gameObject.AddComponent<TypewriterEffect>();
                mTypewriterEffect.charsPerSecond = TypewriterCharPerSec;
                mTypewriterEffect.fadeInTime = TypewriterCharFadeIn;
                mTypewriterEffect.keepFullDimensions = true;
            }
        }

        public void show(bool fading = true)
        {
            if (TypewriterEnabled && mTypewriterEffect == null)
            {
                updateTypewriter();
            }

            //preparations
            switch (mCurrentDialogue.DialogueType)
            {
                case DialogueBehaviour.Type.Information:
                    DialogueLogo.GetComponent<UISprite>().spriteName = DialogueSpriteForward;
                    showMain(fading);
                    break;

                case DialogueBehaviour.Type.Persistant:
                    DialogueLogo.GetComponent<UISprite>().spriteName = DialogueSpriteLocked;
                    showMain(fading);
                    break;

                case DialogueBehaviour.Type.Decision:
                    prepareDecision();
                    //select the new decision dialogue to fade in instead of the global dialogue
                    showDecision(fading);
                    break;

                case DialogueBehaviour.Type.Block:
                    DialogueLogo.GetComponent<UISprite>().spriteName = DialogueSpriteForward;
                    prepareBlock();
                    showMain(fading);
                    break;
            }
        }

        private void enableBackgroundTexture()
        {
            //ensure blocking texture alpha is zero before enable
            BlockingTexture.GetComponent<UITexture>().alpha = 0;

            NGUITools.SetActive(BlockingTexture, true);

            //show blocking texture
            TweenAlpha.Begin(BlockingTexture, TweenDuration, 0.7f);
        }

        private void disableBackgroundTexture()
        {
            //ensure blocking texture alpha is at maximum defined value before tweening
            BlockingTexture.GetComponent<UITexture>().alpha = 0.7f;

            //hide blocking texture
            TweenAlpha.Begin(BlockingTexture, TweenDuration, 0);
        }

        private void prepareBlock()
        {
            enableBackgroundTexture();

            //move dialogue nearer to the screen center
            mOrigContentBottomMargin = !mOrigContentBottomMargin.HasValue ? ContentTransform.GetComponent<UILabel>().bottomAnchor.absolute : mOrigContentBottomMargin;
            ContentTransform.GetComponent<UILabel>().bottomAnchor.Set(GuiMainTransform, 0, (float)mOrigContentBottomMargin + AdditionalBottomBlockMarginInPercent * Screen.height);
            ContentTransform.GetComponent<UILabel>().ResetAndUpdateAnchors();
        }

        private void prepareDecision()
        {
            enableBackgroundTexture();

            //set the fields of the seperate decision dialogue
            HeaderLabelDecision.text = HeaderLabelMain.text;
            ContentLabelDecision.text = ContentLabelMain.text;
            PortraitSpriteDecision.spriteName = PortraitSpriteMain.spriteName;
        }

        public void progress(DialogueBehaviour dialogue)
        {
            List<GUIMessage> messages = new List<GUIMessage>();

            if (mCurrentDialogue == null)
                return;

            //currently progressed dialog is the same as the user's dialogue to progress
            if (mCurrentDialogue.getInternalGuid() == dialogue.getInternalGuid())
            {
                progress();
            }
            else
            {
                messages.Add(new GUIMessage("DialogeBehaviour", GUIMessage.Type.WARNING, string.Format("Dialogue interrupted")));
                setDialogue(dialogue);
            }

            if (messages.Count > 0)
            {
                //notify gui that messages occured
                if (onDialogueGuiMessagesOccured != null)
                {
                    onDialogueGuiMessagesOccured(messages);
                }
            }
        }

        //ngui event delegate triggers this method by onClick!
        public void progressByGuiTrigger()
        {
            if (mCurrentDialogue.DialogueType != DialogueBehaviour.Type.Persistant
                && mCurrentDialogue.DialogueType != DialogueBehaviour.Type.Decision)
            {
                progress();
            }
        }

        public void declineByGuiTrigger()
        {
            if (!base.getInputLocked())
            {
                base.lockInput();
                hide();
                sendDecisionEvent(false);
            }
        }

        public void confirmByGuiTrigger()
        {
            if (!base.getInputLocked())
            {
                base.lockInput();
                hide();
                sendDecisionEvent(true);
            }
        }

        #region Show Logic

        private void fireBeforeShowTweenStartedEvent()
        {
            if (beforeShowTweenStarted != null)
            {
                beforeShowTweenStarted(mCurrentDialogue);
            }
        }
        private void fireAfterHideTweenFinishedEvent()
        {
            if (afterHideTweenFinished != null)
            {
                afterHideTweenFinished(mCurrentDialogue);
            }
        }

        //main block
        public void onDialogueMainTweenFinished()
        {
            if (mMainHidden)
            {
                finalizeHideMain();
            }
            else
            {
                finalizeShowMain();
            }
        }

        private void finalizeShowMain()
        {
            if (!mMainFinalizeHappend)
            {
                mMainFinalizeHappend = true;
                TweenMain.Sample(1.0f, true);

                stopTweenMain();
            }
        }
        private void finalizeHideMain(bool suppressEvent = false)
        {
            if (!mMainFinalizeHappend)
            {
                mMainFinalizeHappend = true;
                TweenMain.Sample(0.0f, true);

                stopTweenMain();

                //do stuff after a blocking dialogue was shown
                if (mCurrentDialogue.DialogueType == DialogueBehaviour.Type.Block)
                    afterHideBlock();

                NGUITools.SetActive(DialogueMain, false);

                if (!suppressEvent)
                    fireAfterHideTweenFinishedEvent();
            }
        }

        private void startTweenMain(bool forward = true)
        {
            TweenMain.enabled = true;


            /* TODO:
             * Implement this tweenFactor thing for decision too!
             * Test show with tween and hide without
             * 
             */

            if (forward)
                TweenMain.tweenFactor = 0.0f;
            else
                TweenMain.tweenFactor = 1.0f;

            TweenMain.Play(forward);
            TweenMain.ResetToBeginning();
        }

        private void stopTweenMain()
        {
            TweenMain.enabled = false;
        }

        private void showMain(bool fade = true)
        {
            if (!mMainHidden)
                return;

            if (!mMainFinalizeHappend)
            {
                //Instanly finalize the previous operation
                finalizeHideMain();
            }

            mMainHidden = false;

            //NGUITools.SetActive(DialogueLogo, true);
            NGUITools.SetActive(DialogueMain, true);
            mMainFinalizeHappend = false;

            fireBeforeShowTweenStartedEvent();
            if (fade)
            {
                startTweenMain();
            }
            else
            {
                finalizeShowMain();
            }
        }

        private void hideMain(bool fade = true, bool suppressEvent = false)
        {
            if (mMainHidden)
                return;

            if (!mMainFinalizeHappend)
            {
                //Instanly finalize the previous operation
                finalizeShowMain();
            }

            mMainHidden = true;
            mMainFinalizeHappend = false;

            if (fade)
            {
                startTweenMain(false);
            }
            else
            {
                finalizeHideMain(suppressEvent);
            }
        }

        //decision block
        public void onDialogueDecisionTweenFinished()
        {
            if (mDecisionHidden)
            {
                finalizeHideDecision();
            }
            else
            {
                finalizeShowDecision();
            }
        }

        private void finalizeShowDecision()
        {
            if (!mDecisionFinalizeHappend)
            {
                mDecisionFinalizeHappend = true;
                TweenDecision.Sample(1.0f, true);

                stopTweenDecision();
            }
        }
        private void finalizeHideDecision(bool suppressEvent = false)
        {
            if (!mDecisionFinalizeHappend)
            {
                mDecisionFinalizeHappend = true;
                TweenDecision.Sample(0.0f, true);

                stopTweenDecision();

                //do stuff after decision is shown
                afterHideDecision();

                NGUITools.SetActive(DialogueDecision, false);

                if (!suppressEvent)
                    fireAfterHideTweenFinishedEvent();
            }
        }

        private void startTweenDecision(bool forward = true)
        {
            TweenDecision.enabled = true;

            if (forward)
                TweenDecision.tweenFactor = 0.0f;
            else
                TweenDecision.tweenFactor = 1.0f;

            TweenDecision.Play(forward);
            TweenDecision.ResetToBeginning();
        }

        private void stopTweenDecision()
        {
            TweenDecision.enabled = false;
        }

        private void showDecision(bool fade = true)
        {
            if (!mMainHidden)
            {
                hideMain(false, false);
            }

            if (!mDecisionHidden)
                return;

            if (!mDecisionFinalizeHappend)
            {
                //Instanly finalize the previous operation
                finalizeHideDecision();
            }

            mDecisionHidden = false;

            NGUITools.SetActive(DialogueLogo, true);
            NGUITools.SetActive(DialogueDecision, true);
            mDecisionFinalizeHappend = false;

            fireBeforeShowTweenStartedEvent();
            if (fade)
            {
                startTweenDecision();
            }
            else
            {
                finalizeShowDecision();
            }
        }

        private void hideDecision(bool fade = true, bool supressEvent = false)
        {
            if (mDecisionHidden && fade)
                return;

            if (!mDecisionFinalizeHappend)
            {
                //Instanly finalize the previous operation
                finalizeShowDecision();
            }

            mDecisionHidden = true;
            mDecisionFinalizeHappend = false;

            if (fade)
            {
                startTweenDecision(false);
            }
            else
            {
                finalizeHideDecision(supressEvent);
            }
        }

        public void hideWithoutTweenAndEvent()
        {
            if (mCurrentDialogue == null)
            {
                mIsLocked = false;
                return;
            }

            if (mCurrentDialogue.DialogueType == DialogueBehaviour.Type.Decision)
            {
                hideDecision(false, true);
            }
            else
            {
                hideMain(false, true);
            }

            mIsLocked = false;
        }

        #endregion

        private void sendDecisionEvent(bool status)
        {
            if (onDialogueDecision != null)
            {
                onDialogueDecision(mCurrentDialogue, status);
            }
        }

        public void progress()
        {
            if (!base.getInputLocked())
            {
                base.lockInput();
                if (mCurrentDialogue.DialogueType != DialogueBehaviour.Type.Decision)
                {
                    updateDialogue();
                    if (onDialogueProgressed != null && !mIsProgressed)
                    {
                        onDialogueProgressed(mCurrentDialogue);
                    }
                    if (mIsProgressed)
                        hide();
                }
            }
        }

        public void hide(bool fading = true)
        {
            switch (mCurrentDialogue.DialogueType)
            {
                case DialogueBehaviour.Type.Information:
                    StopCoroutine("fadingHide");
                    hideMain(fading);
                    break;
                case DialogueBehaviour.Type.Persistant:
                    hideMain(fading);
                    break;
                case DialogueBehaviour.Type.Block:
                    hideMain(fading);
                    break;
                case DialogueBehaviour.Type.Decision:
                    hideDecision(fading);
                    break;
            }

            mIsLocked = false;
        }

        public void hideAll(bool fading = true)
        {

            if (mCurrentDialogue != null)
                hide(fading);
        }

        private IEnumerator fadingHide(float time)
        {
            yield return new WaitForSeconds(time);
            hide();
        }

        private void afterHideBlock()
        {
            disableBackgroundTexture();

            ContentTransform.GetComponent<UILabel>().bottomAnchor.Set(GuiMainTransform, 0, (float)mOrigContentBottomMargin);
            ContentTransform.GetComponent<UILabel>().ResetAndUpdateAnchors();
        }

        private void afterHideDecision()
        {
            disableBackgroundTexture();
        }
    }
}
