using UnityEngine;
using System.Collections.Generic;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// This class maps sprites to a set of interaction types so the hint user can request sprites by using the InteractionType enum
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(UIAtlas))]
    public class InteractionSpriteListBehaviour : MonoBehaviour
    {
        //atlas containing all possible sprites
        public UIAtlas Atlas;

        //this enum contains all possible interaction types
        public enum InteractionType
        {
            NONE,
            SHAKE,
            FIND_LOST_MARKER,
            WAIT,
            SWIPE,
            ROTATE_OBJECT,
            DRAW_LINE,
            TAP,
            STEP_BACK,
            E_COLLECTOR,
            E_MIRROR_1,
            E_MIRROR_2,
            E_TANK
        }

        //holds the mapping enum value - sprite defined in the editor
        [HideInInspector]
        public UISpriteData[] Mapping;

        //contains the seleted indices for each enum type
        [HideInInspector]
        public int[] Selected = null;

        #region Unity Messages

        void Start()
        {
            if (this.Atlas == null)
            {
                Atlas.GetComponent<UIAtlas>();
            }
        }
        #endregion

        /// <summary>
        /// returns the UISpriteData for the requested interaction type
        /// </summary>
        /// <param name="iaType">the interaction type requested</param>
        /// <returns>UISpriteData for the requested interaction type</returns>
        public UISpriteData getSpriteFor(InteractionType iaType)
        {
            string spriteRequestName = Mapping[(int)iaType].name;

            //small hack for the "NONE" icon
            if (iaType == InteractionType.NONE)
                return Atlas.GetSprite("");

            return Atlas.GetSprite(spriteRequestName);
        }
    }
}
