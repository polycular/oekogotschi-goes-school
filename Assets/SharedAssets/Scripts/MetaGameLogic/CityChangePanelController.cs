using UnityEngine;
using System.Collections;


namespace SharedAssets.MetaGameLogic
{
    public class CityChangePanelController : MonoBehaviour
    {
        public enum CurrentText
        {
            BAD,
            AFTER_LIGHTRAY,
            AFTER_MAZE,
            AFTER_WASTEFISHING,
            AFTER_CYCLE,
            OPEN_WORLD
        }

        public UILabel TextField;

        public string TextBad;
        public string TextLightray;
        public string TextMaze;
        public string TextWasteFishing;
        public string TextCycle;
        public string TextOpenWorld;

        private CurrentText mCurrentState;


        void Start()
        {

        }


        void Update()
        {

        }

        public void setCurrentText(CurrentText state)
        {
            this.mCurrentState = state;

            switch (mCurrentState)
            {
                case CurrentText.BAD:
                    TextField.text = TextBad;
                    break;

                case CurrentText.AFTER_LIGHTRAY:
                    TextField.text = TextLightray;
                    break;

                case CurrentText.AFTER_MAZE:
                    TextField.text = TextMaze;
                    break;

                case CurrentText.AFTER_WASTEFISHING:
                    TextField.text = TextWasteFishing;
                    break;

                case CurrentText.AFTER_CYCLE:
                    TextField.text = TextCycle;
                    break;

                case CurrentText.OPEN_WORLD:
                    TextField.text = TextOpenWorld;
                    break;

                default:
                    break;
            }
        }
    }
}
