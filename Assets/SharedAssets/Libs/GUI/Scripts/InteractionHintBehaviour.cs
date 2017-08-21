using System;
using UnityEngine;

namespace SharedAssets.GuiMain
{
    public class InteractionHintBehaviour : MonoBehaviour, IHint<UISpriteData>
    {

        public UISprite HintBackground;
        public UISprite HintPulse;
        public UISprite InteractionIcon;
        public HintStyleBehaviour DefaultStyle;
        public InteractionSpriteListBehaviour SpriteList;

        public bool DebugPulse = false;
        public float PulseTimeInSeconds = 1.0f;
        public float MinPulseScale = 0.5f;
        public float MaxPulseScale = 1.0f;


        private float mPulseProgress = 0.0f;

        /// <summary>
        /// The current UISprite of the hint.
        /// </summary>
        public UISpriteData Content { get; set; }

        /// <summary>
        /// Whether the hint is visible. 
        /// </summary>
        public bool IsVisible { get; set; }

        public bool IsPulsing { private set; get; }

        public Color BackgroundColor { get; set; }

        public Color ContentColor { get; set; }

        #region PUBLIC_METHODS

        public bool setContent(UISpriteData content, bool visible = true, bool pulse = true)
        {
            //is false if a pulse was intended but another pull was already active
            bool pulsesAsIntended = true;

            Content = content;
            IsVisible = visible;

            if (IsPulsing && pulse)
                pulsesAsIntended = false;

            else if (!IsPulsing && pulse)
                this.pulse();

            return pulsesAsIntended;
        }

        public void resetColors()
        {
            this.BackgroundColor = DefaultStyle.DefaultBgColor;
            this.ContentColor = DefaultStyle.DefaultContentColor;
        }

        public bool pulse()
        {
            if (HintPulse == null || IsPulsing)
                return false;

            IsPulsing = true;
            HintPulse.color = new Color(HintPulse.color.r, HintPulse.color.g, HintPulse.color.b, 1.0f);
            return true;
        }

        #endregion


        private void resetPulse()
        {
            IsPulsing = false;
            mPulseProgress = 0.0f;
            HintPulse.transform.localScale = new Vector3(MinPulseScale, MinPulseScale, MinPulseScale);
            HintPulse.color = new Color(HintPulse.color.r, HintPulse.color.g, HintPulse.color.b, 0.0f);
        }


        #region UNITY_METHODS
        void Awake()
        {
            resetColors();
        }

        void Update()
        {
            if (DebugPulse)
            {
                pulse();
                DebugPulse = false;
            }
            updateUIElement();
        }
        #endregion

        private void updateUIElement()
        {
            //update content
            if (InteractionIcon != null)
            {
                if (Content == null)
                {
                    NGUITools.SetActive(InteractionIcon.gameObject, false);
                }
                else
                {
                    InteractionIcon.spriteName = Content.name;
                }      
            }

            //update visibility
            if (HintBackground != null)
            {
                NGUITools.SetActive(HintBackground.gameObject, IsVisible);
            }
            if (InteractionIcon != null && Content != null)
            {
                NGUITools.SetActive(InteractionIcon.gameObject, IsVisible);
            }
            if (HintPulse != null)
                NGUITools.SetActive(HintPulse.gameObject, IsVisible);

            //update color
            if (HintBackground != null)
                HintBackground.color = BackgroundColor;
            if (InteractionIcon != null)
                InteractionIcon.color = ContentColor;


            //update pulse progress
            if (IsPulsing)
            {
                mPulseProgress += Time.deltaTime / PulseTimeInSeconds;

                mPulseProgress = Mathf.Clamp01(mPulseProgress);

                if (mPulseProgress == 1.0f)
                {
                    resetPulse();
                }
                else
                {
                    float newScale = MinPulseScale + mPulseProgress * (MaxPulseScale - MinPulseScale);
                    HintPulse.transform.localScale = new Vector3(newScale, newScale, newScale);
                    HintPulse.color = new Color(HintPulse.color.r, HintPulse.color.g, HintPulse.color.b, 1.0f - mPulseProgress);
                }

            }
        }
    }
}