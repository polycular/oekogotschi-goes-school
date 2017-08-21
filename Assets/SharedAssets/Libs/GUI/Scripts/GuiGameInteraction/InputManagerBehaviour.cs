using UnityEngine;
using System.Collections;

namespace GuiMain.GuiInteraction
{
    public class InputManagerBehaviour : MonoBehaviour
    {
        private bool mLastWasDown = false;
        private bool mCurrentIsDown = false;
        private bool mLastWasPassing = false;
        private bool mCurrentIsPassing = false;

        //last 'known' position - excludes position on GUI elements
        private Vector3 mLastScreenPosition;

        #region UnityMethods

        void Start()
        {
            Update();
        }

        void Update()
        {
            mLastWasPassing = mCurrentIsPassing;
            mLastWasDown = mCurrentIsDown;
            mCurrentIsDown = Input.GetMouseButton(0);
            mCurrentIsPassing = (UICamera.hoveredObject == null || UICamera.hoveredObject.GetComponent<UIRoot>() != null);
            if (passedFinger())
            {
                mLastScreenPosition = Input.mousePosition;
            }
        }

        #endregion

        /// <summary>
        /// Returns true when the finger/mouse has begung tocuhing/clicking in this frame AND is not obstructed by the main gui
        /// Beware that you cann miss passFingerDown() but still get passFinger(), if a "drag" has begun over the GUI
        /// </summary>
        /// <returns></returns>
        public bool passedFingerDown()
        {
            return (mCurrentIsDown && !mLastWasDown && mCurrentIsPassing);
        }

        /// <summary>
        /// Returns if the finger/mouse is not obstructed by GUI anymore, but was in the previous frame.
        /// Only returns true if the finger/mouse is pressed down
        /// </summary>
        /// <returns></returns>
        public bool passedFingerEnter()
        {
            return (mCurrentIsDown && !mLastWasPassing && mCurrentIsPassing);
        }

        /// <summary>
        /// Returns if the finger/mouse is obstructed by GUI now, but was not in the previous frame.
        /// Only returns true if the finger/mouse is pressed down
        /// </summary>
        /// <returns></returns>
        public bool passedFingerLeave()
        {
            return (mCurrentIsDown && mLastWasPassing && !mCurrentIsPassing);
        }

        /// <summary>
        /// Returns true if thefinger/mpouse has ben lifted above non-obstructed space
        /// </summary>
        /// <returns></returns>
        public bool passedFingerUp()
        {
            return (!mCurrentIsDown && mLastWasDown && mCurrentIsPassing);
        }

        /// <summary>
        /// Returns true as long as the finger/mouse is in a pressed down state, and not obstructed by GUI
        /// </summary>
        /// <returns></returns>
        public bool passedFinger()
        {
            return (mCurrentIsDown && mCurrentIsPassing);
        }

        public Vector3 getLastPassedFingerOnScreen()
        {
            return mLastScreenPosition;
        }

        public Ray getLastPassedFingerRay(Camera cam)
        {
            Ray r = cam.ScreenPointToRay(mLastScreenPosition);
            return r;
        }
    }
}
