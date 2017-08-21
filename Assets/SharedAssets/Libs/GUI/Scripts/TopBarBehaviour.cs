using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    public class TopBarBehaviour : MonoBehaviour
    {
        //sprites used for the top bar (points / countdown)
        //NOTE: if you don't need both, just leave the fields null
        public Sprite SpriteLeft;
        public Sprite SpriteRight;
        public string LabelLeft;
        public string LabelRight;
        public Sprite Avatar;

        //debug
        public bool DebugShow;

        #region UnityMethods

        private void Update()
        {
            if (DebugShow)
            {
                DebugShow = false;
                update();
            }
        }

        #endregion

        public void update()
        {
            //TODO: refactor "show" to "send" ?
            //ignore naming .show()
            GUIManagerBehaviour.Instance.show(this);
        }

    }
}
