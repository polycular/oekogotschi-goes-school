using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// The HintManager allows easy access to different Hint Types that are
    /// available the GUI
    /// </summary>
    public class HintManagerBehaviour : MonoBehaviour
    {
        public TextHintBehaviour TextHint;
        public InteractionHintBehaviour InteractionHint;

        /// <summary>
        /// Configures a combined hint, consisting out of a text hint and an interaction hint with a hint icon.
        /// </summary>
        /// <param name="text">The text that should be displayed in the text hint.</param>
        /// <param name="sprite">The sprite that should be displayed as an icon in the interaction hint.</param>
        /// <param name="visibility">Whether the hint should be set visible. </param>
        /// <param name="pulse">Whether the hint should pulse to attract attention. 
        /// Note: If the hint is already pulsing when this method is called, 
        /// there will be no additional pulse and the method will return false. </param>
        /// <returns>Whether the pulse was successfully triggered. </returns>
        /// <returns></returns>
        public bool setCombinedHint(string text, UISpriteData sprite, bool visibility = true, bool pulse = true)
        {
            resetCombinedHintStyle();

            bool pulseFailedText = false;
            bool pulseFailedIA = false;

            //if one of the pulse methods fail, return false
            pulseFailedText = TextHint.setContent(text, visibility, pulse);
            pulseFailedIA = InteractionHint.setContent(sprite, visibility, pulse);

            return pulseFailedText && pulseFailedIA;
        }

        public bool setCombinedHint(string text, InteractionSpriteListBehaviour.InteractionType interactionHint, bool visibility = true, bool pulse = true)
        {
            return setCombinedHint(text, InteractionHint.SpriteList.getSpriteFor(interactionHint), visibility, pulse);
        }

        /// <summary>
        /// Call this AFTER setCombinedHint to flag it as warning
        /// </summary>
        public void setCombinedHintAsWarning()
        {
            TextHint.ContentColor = TextHint.HintStyleBehaviour.WarnContentColor;
            TextHint.BackgroundColor = TextHint.HintStyleBehaviour.WarnBgColor;
            InteractionHint.ContentColor = InteractionHint.DefaultStyle.WarnContentColor;
            InteractionHint.BackgroundColor = InteractionHint.DefaultStyle.WarnBgColor;
        }
        public void resetCombinedHintStyle()
        {
            TextHint.resetColors();
            InteractionHint.resetColors();
        }

    }
}