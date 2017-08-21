using UnityEngine;
using System.Collections;
using System;

namespace SharedAssets
{
    public class EggTimerBehaviour : MonoBehaviour
    {
        public delegate void OnFinishedHandler(EggTimerBehaviour eggtimer);
        public OnFinishedHandler onFinished;

		public float TimeLimit = 0f;

		public float TotalSecondsRecorded
        {
            get
            {
                return mTotalSecondsRecorded;
            }
        }
		
        public float RemainingSeconds
        {
            get
            {
                return Mathf.Max(0f, TimeLimit - mTotalSecondsRecorded);
            }
        }

        public TimeSpan RemainingTimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds(RemainingSeconds);
            }
        }
		
        public DateTime RemainingDateTime
        {
            get
            {
                return new DateTime(RemainingTimeSpan.Ticks);
            }
        }

      
        public float RemainingSecondsOverdue
        {
            get
            {
                return TimeLimit - mTotalSecondsRecorded;
            }
        }
        
      
        public TimeSpan RemainingTimeSpanOverdue
        {
            get
            {
                return TimeSpan.FromSeconds(RemainingSecondsOverdue);
            }
        }

  
        public float RemainingTime01
        {
            get
            {
                return Mathf.InverseLerp(TimeLimit, 0f, mTotalSecondsRecorded);
            }
        }

        public string CountdownString
        {
            get
            {
                return RemainingDateTime.ToString(@"mm\:ss");
            }
        }

        public float RemainingSecondsP1
        {
            get
            {
                return Mathf.Min(TimeLimit, Mathf.Max(0f, TimeLimit - (mTotalSecondsRecorded-1f)));
            }
        }

        public TimeSpan RemainingTimeSpanP1
        {
            get
            {
                return TimeSpan.FromSeconds(RemainingSecondsP1);
            }
        }

        public DateTime RemainingDateTimeP1
        {
            get
            {
                return new DateTime(RemainingTimeSpanP1.Ticks);
            }
        }

        public string CountdownStringP1
        {
            get
            {
                return RemainingDateTimeP1.ToString(@"mm\:ss");
            }
        }

        public bool IsFinished
        {
            get
            {
                return mTotalSecondsRecorded >= TimeLimit;
            }
        }

        public bool HasFinishedNow
        {
            get
            {
                return !mHasFinishedBefore && IsFinished;
            }
        }

   
        public void restart()
        {
            resetRecording();
        }


        public void pause()
        {
            this.enabled = false;
        }
		
        public void unpause()
        {
            this.enabled = true;
        }

		protected float mTotalSecondsRecorded = 0f;

        protected bool mHasFinishedBefore = false;
       
        virtual protected void resetRecording()
        {
            mTotalSecondsRecorded = 0f;
            mHasFinishedBefore = false;
        }

        virtual protected void progressTime()
        {
            mTotalSecondsRecorded += Time.deltaTime;
        }
		
        void Start()
        {
            resetRecording();
        }
		
        void Update()
        {
            if (IsFinished)
            {
                mHasFinishedBefore = true;
            }

            progressTime();
            
            if (HasFinishedNow)
            {
                if (onFinished != null)
                {
                    onFinished(this);
                }
            }
        }
    }
}