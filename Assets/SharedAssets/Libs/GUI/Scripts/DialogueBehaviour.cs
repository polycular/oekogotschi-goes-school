using System;
using UnityEngine;
using System.Collections.Generic;

namespace SharedAssets.GuiMain
{
    public class DialogueBehaviour : MonoBehaviour
    {
        public enum Type
        {
            Information,
            Block,
            Persistant,
            Decision
        }

        public List<string> Header;
        //[TextArea(3,10)] - not working with FullInspectorPlugin
        public List<string> Content;
        public List<Sprite> Portrait;
		
        public Type DialogueType;
		
        public float FadingTime;
        
        //must be a single dialogue! / type must be "Block" for definition.
        public bool IsQuest;

        //debug
        public bool DebugShow;
        public bool DebugProgress;
        public bool DebugPlayOnStart;

        private readonly Guid mInternalGuid = Guid.NewGuid();

        #region UnityMethods

        private void Update()
        {
            if (DebugPlayOnStart)
            {
                DebugShow = true;
                DebugPlayOnStart = false;
            }
            if (DebugShow)
            {
                DebugShow = false;
                show();
            }
            if (DebugProgress)
            {
                DebugProgress = false;
                progress();
            }
        }

        #endregion

        public Guid getInternalGuid()
        {
            return mInternalGuid;
        }

        public void show(bool disableTween = false)
        {
            GUIManagerBehaviour.Instance.show(this, !disableTween);
        }

        //get the next content / header / portrait in the dialogue
        public void progress()
        {
            GUIManagerBehaviour.Instance.progress(this);
        }


        public void hide(bool disableTween = false)
        {
            if(GUIManagerBehaviour.Instance != null)
                GUIManagerBehaviour.Instance.hide(this, !disableTween);
        }
    }
}