using UnityEngine;
using System.Collections;
using System.Security.Permissions;

namespace SharedAssets.GuiMain
{
    public class TextHintBehaviour : MonoBehaviour, IHint<string>
    {
        public string Content { get; set; }
        public bool IsVisible { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ContentColor { get; set; }
        public bool IsPulsing { get; private set; }

        public bool DebugPulse = false;
        public float PulseTimeInSeconds = 1.0f;
        public float MinPulseScaleY = 0.5f;
        public float MaxPulseScaleY = 1.2f;

        public HintStyleBehaviour HintStyleBehaviour;

        public UISprite SpriteBg;
        public UISprite SpritePulse;
        public UILabel LabelContent;

        private float mPulseProgress = 0.0f;

        #region Unity Messages

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


        public bool setContent(string content, bool visible = true, bool pulse = true)
        {
            //is false if a pulse was intended but another pull was already active
            bool pulsesAsIntended = true;

            this.Content = content;
            this.IsVisible = visible;

            if (IsPulsing && pulse)
                pulsesAsIntended = false;

            else if (!IsPulsing && pulse)
                this.pulse();

            return pulsesAsIntended;
        }

        public void resetColors()
        {
            this.BackgroundColor = HintStyleBehaviour.DefaultBgColor;
            this.ContentColor = HintStyleBehaviour.DefaultContentColor;
        }

        public bool pulse()
        {
            if (SpritePulse == null || IsPulsing)
                return false;

            IsPulsing = true;
            SpritePulse.color = new Color(SpritePulse.color.r, SpritePulse.color.g, SpritePulse.color.b, 1.0f);
            return true;
        }

        private void resetPulse()
        {
            IsPulsing = false;
            mPulseProgress = 0.0f;
            SpritePulse.transform.localScale = new Vector3(SpritePulse.transform.localScale.x, MinPulseScaleY, SpritePulse.transform.localScale.z);
            SpritePulse.color = new Color(SpritePulse.color.r, SpritePulse.color.g, SpritePulse.color.b, 0.0f);
        }

        private void updateUIElement()
        {
            //update visibility
            if (SpriteBg != null)
                NGUITools.SetActive(SpriteBg.gameObject, IsVisible);
            if (SpritePulse != null)
                NGUITools.SetActive(SpritePulse.gameObject, IsVisible);
            if (LabelContent != null)
                NGUITools.SetActive(LabelContent.gameObject, IsVisible);

            //apply current content
            if (LabelContent != null)
                LabelContent.text = Content;

            //apply current colors
            if (SpriteBg != null)
                SpriteBg.color = BackgroundColor;
            if (LabelContent != null)
                LabelContent.color = ContentColor;

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
                    float newYScale = MinPulseScaleY + mPulseProgress * (MaxPulseScaleY - MinPulseScaleY);
                    SpritePulse.transform.localScale = new Vector3(SpritePulse.transform.localScale.x, newYScale,
                        SpritePulse.transform.localScale.z);
                    SpritePulse.color = new Color(SpritePulse.color.r, SpritePulse.color.g, SpritePulse.color.b, 1.0f - mPulseProgress);
                }

            }
        }
    }
}
