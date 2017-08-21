using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// Implements hint functionality such as setting the content of a hint and pulsing of the hint to get the attention of the user. 
    /// </summary>
    public interface IHint<T>
    {
        #region PUBLIC_PROPERTIES

        /// <summary>
        /// The content of the hint. This might for example be a text, image etc.
        /// </summary>
        T Content
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the hint is being displayed right now. 
        /// </summary>
        bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// The background color of the hint. 
        /// </summary>
        UnityEngine.Color BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// The color of the hint's content.
        /// </summary>
        UnityEngine.Color ContentColor
        {
            get;
            set;
        }
        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Sets the content of the hint. 
        /// </summary>
        /// <param name="content">The hint's content. </param>
        /// <param name="visible">Whether the hint should be set visible. </param>
        /// <param name="pulse">Whether the hint should pulse to attract attention. 
        /// Note: If the hint is already pulsing when this method is called, 
        /// there will be no additional pulse and the method will return false. </param>
        /// <returns>Whether the pulse was successfully triggered. </returns>
        bool setContent(T content, bool visible = true, bool pulse = true);

        /// <summary>
        /// Resets the background and content colors to default.
        /// </summary>
        void resetColors();

        /// <summary>
        /// Triggers a pulse motion of the hint to attract the user's attention.
        /// Note: If the hint is already pulsing when this method is called, 
        /// there will be no additional pulse and the method will return false.
        /// </summary>
        /// <returns>Whether the pulse was successfully triggered.</returns>
        bool pulse();

        #endregion

    }
}