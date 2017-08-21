using UnityEngine;
using System.Collections;

namespace SharedAssets
{
    /// <summary>
    /// This class interacts with the Unity Vibration API
    /// Different vibration modes are supported : once / timed / endless
    /// </summary>
    public class VibrationBehaviour : MonoBehaviour
    {
        public float VibrationDurationInSec = 1.0f;

        private float mVibrationDuration = 0f;

        private bool mIsEndless = false;
        private bool mIsTimed = false;

        //used for workaround in update(float deltaTime) because 0.4 sec is the shortest vibration possible
        private float mMinimumVibrationDuration = 0.4f;


        #region Unity Methods

        // Update is called once per frame
        void Update()
        {
            update();
            update(Time.deltaTime);
        }

        #endregion

        private void startEndless()
        {
            mIsTimed = false;
            mIsEndless = true;
        }

        private void cancelEndless()
        {
            mIsEndless = false;
        }

        private void startTimed(float durationInSec)
        {
            mVibrationDuration = durationInSec;

            mIsEndless = false;
            mIsTimed = true;
        }

        private void cancelTimed()
        {
            mIsTimed = false;
        }

        /// <summary>
        /// Update function for endless vibration
        /// </summary>
        private void update()
        {
            if (mIsEndless)
            {
                Handheld.Vibrate();
            }
        }

        /// <summary>
        /// Update function for timed vibration
        /// </summary>
        /// <param name="deltaTime">Delta time in seconds</param>
        private void update(float deltaTime)
        {
            if (mIsTimed)
            {
                //workaround because one Vibration lasts about 0.4 seconds
                if (mVibrationDuration - mMinimumVibrationDuration < 0)
                {
                    vibrateOnce();
                    mIsTimed = false;
                }
                else
                {
                    mVibrationDuration -= deltaTime;

                    if (mVibrationDuration <= 0)
                    {
                        mIsTimed = false;
                    }
                    Handheld.Vibrate();
                }
            }
        }

        /// <summary>
        /// Vibrates once ( ~ 0.4 sec )
        /// </summary>
        public void vibrateOnce()
        {
            Handheld.Vibrate();
        }

        /// <summary>
        /// Vibrates as long as specified through field: 'VibrationDurationInSec'
        /// </summary>
        public void vibrate()
        {
            startTimed(VibrationDurationInSec);
        }

        /// <summary>
        /// Vibrates until 'cancelVibration()' is invoked
        /// </summary>
        public void vibrateEndless()
        {
            startEndless();
        }

        public void cancelVibration()
        {
            //cancel all vibration modes

            cancelEndless();
            cancelTimed();
        }
    }
}
