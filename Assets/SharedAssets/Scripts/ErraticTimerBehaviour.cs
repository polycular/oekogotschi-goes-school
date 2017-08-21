using UnityEngine;
using System.Collections;
using System;

namespace SharedAssets
{
    /// <summary>
    /// An ErraticTimers is kind a "broken" EggTimer
    /// Its time does not flow linear but in random directions, so the remaining Time
    /// may even go up
    /// The chance for it to go down decreases as the real tracked reaches the TimeLimit
    /// </summary>
    public class ErraticTimerBehaviour : EggTimerBehaviour
    {

        public float RealTotalSecondsRecorded
        {
            get
            {
                return mRealTotalSecondsRecorded;
            }
        }

        public float RealRemainingTime01
        {
            get
            {
                return Mathf.InverseLerp(TimeLimit, 0f, mRealTotalSecondsRecorded);
            }
        }

        /// <summary>
        /// Example:
        /// Jerkiness 1f & Stability 1f is linear behaviour (EggTimer)
        /// </summary>
        [Range(0f, 1f), Tooltip("Jerkiness defines the strength countdown changes. Higher Jerkines results in more (if instable) shaky behaviour")]
        public float Jerkiness = 0.1f;

        [Tooltip("A Higher stability means that the chance that the Timer goes back up is lower.")]
        public float Stability = 0.5f;

        public bool AutoStabilize = true;
        public float AutoStabilizeAfterSeconds = 0f;
        public float AutoStabilizeStrength = 1f;

        private float mRealTotalSecondsRecorded = 0f;
        private float mProgressDirection = 0f;

        // Update is called once per frame
        override protected void progressTime()
        {
            mRealTotalSecondsRecorded += Time.deltaTime;

            if (IsFinished)
                return;

            //Go up or down, randomly
            float directionChange = 1f;
            //Only countUp if there is any recorded time to reduce
            if (mTotalSecondsRecorded > 0)
            {
                float chanceCountUp = 1f - (Stability + (
                    (AutoStabilize) ?
                    Mathf.Max(0f, mRealTotalSecondsRecorded - AutoStabilizeAfterSeconds) * AutoStabilizeStrength :
                    0f));
                float dice = UnityEngine.Random.value;

                if (dice < chanceCountUp)
                {
                    directionChange = -1f;
                }
            }

            mProgressDirection = Mathf.Min(1f, Mathf.Max(-1f, mProgressDirection + (directionChange * Mathf.Clamp01(Jerkiness))));

            mTotalSecondsRecorded = Mathf.Max(0f, mTotalSecondsRecorded + mProgressDirection * Time.deltaTime);

        }

        override protected void resetRecording()
        {
            mRealTotalSecondsRecorded = 0f;
            base.resetRecording();
        }
    }
}