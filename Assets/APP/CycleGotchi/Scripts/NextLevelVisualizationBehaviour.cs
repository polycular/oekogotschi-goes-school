using UnityEngine;
using System.Collections;


namespace CycleGotchi
{
    /// <summary>
    /// Special class tha visualizes the "Next Level" Blend-in effect
    /// </summary>
    [RequireComponent(typeof(UI2DSprite))]
    public class NextLevelVisualizationBehaviour : MonoBehaviour
    {

        public float DurationInSeconds = 1f;
        public float GrowSpeed = 3f;


        UI2DSprite mSprite = null;
        bool mIsPlaying = false;
        float mPlayingSince = Mathf.NegativeInfinity;

        void Awake()
        {
            mSprite = this.GetComponent<UI2DSprite>();
        }

        void OnEnable()
        {
            //Restarts and plays the Effect

            mIsPlaying = true;
            mPlayingSince = Time.time;

            mSprite.enabled = true;
        }

        void Update()
        {
            if (mIsPlaying)
            {
                float totProgress = (Time.time - mPlayingSince) / DurationInSeconds;
                float progress = Mathf.Min(1f, totProgress);
                mSprite.alpha = 1f - progress;
                mSprite.transform.localScale = Vector3.one * Mathf.Min(1f, progress * GrowSpeed);
                if (totProgress > 1f)
                {
                    //End the effect
                    mIsPlaying = false;
                    mSprite.enabled = false;
                }
            }
        }

        void OnDisable()
        {
        }
    }
}