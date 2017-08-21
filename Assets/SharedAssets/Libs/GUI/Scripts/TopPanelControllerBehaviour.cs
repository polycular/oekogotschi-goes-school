using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    public class TopPanelControllerBehaviour : PanelController
    {
        public event OnGuiMessagesOccuredHandler onTopBarGuiMessagesOccured;

        //the two sprites at the top bar of the gui
        //hint: you generally want to select points/countdown symbols
        public UISprite TopSpriteLeft;
        public UISprite TopSpriteRight;
        public UILabel TopLabelLeft;
        public UILabel TopLabelRight;
        public UISprite TopAvatarSprite;

        public string DefaultTopSprite;
        public Sprite DefaultAvatarSprite;

        #region UnityMethods

        void Awake()
        {
            TopSpriteLeft.spriteName = DefaultTopSprite;
            TopSpriteRight.spriteName = DefaultTopSprite;
        }

        #endregion


        public void updateTopBar(TopBarBehaviour topbar)
        {
            List<GUIMessage> messages = new List<GUIMessage>();

            //TODO: implement messages
            // -> sprite + label -> both or nothing!

            if (topbar.SpriteLeft == null || string.IsNullOrEmpty(topbar.LabelLeft))
            {
                TopSpriteLeft.spriteName = DefaultTopSprite;
                TopLabelLeft.text = string.Empty;

                messages.Add(new GUIMessage("TopBarBehaviour", GUIMessage.Type.WARNING, "SpriteLeft and/or LabelLeft are null or empty, both set to default"));
            }
            else
            {
                TopSpriteLeft.spriteName = topbar.SpriteLeft.name;
                TopLabelLeft.text = topbar.LabelLeft;
            }

            if (topbar.SpriteRight == null || string.IsNullOrEmpty(topbar.LabelRight))
            {
                TopSpriteRight.spriteName = DefaultTopSprite;
                TopLabelRight.text = string.Empty;

                //messages.Add(new GUIMessage("TopBarBehaviour", GUIMessage.Type.WARNING, "SpriteRight and/or LabelRight are null or empty, both set to default"));
            }
            else
            {
                TopSpriteRight.spriteName = topbar.SpriteRight.name;
                TopLabelRight.text = topbar.LabelRight;
            }

            if (topbar.Avatar == null)
            {
                TopAvatarSprite.spriteName = DefaultAvatarSprite.name;
                messages.Add(new GUIMessage("TopBarBehaviour",GUIMessage.Type.WARNING, "Avatar sprite is null, Avatar set to default"));
            }
            else
            {
                TopAvatarSprite.spriteName = topbar.Avatar.name;
            }

            if (messages.Count > 0)
            {
                //notify gui that messages occured
                if (onTopBarGuiMessagesOccured != null)
                {
                    onTopBarGuiMessagesOccured(messages);
                }
            }
        }

        public void clearTopBar()
        {
            TopSpriteLeft.spriteName = "";
            TopSpriteRight.spriteName = "";

            TopLabelLeft.text = string.Empty;
            TopLabelRight.text = string.Empty;
        }
    }
}
