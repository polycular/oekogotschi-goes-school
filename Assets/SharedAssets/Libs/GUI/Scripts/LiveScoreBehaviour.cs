using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SharedAssets.GuiMain
{
    /// <summary>
    /// The LiveScoreBehaviour manages visible live (in-game) scores that are rewarded
    /// in screen space.
    /// </summary>
    public class LiveScoreBehaviour : MonoBehaviour
    {
        public GUIHelperBehaviour Helper = null;
        public UIPanel Container = null;
        [Tooltip("How many seconds the score should be kept alive. Relaized over progress speed")]
        public float LiveDuration = 1f;
        [Tooltip("How far does every score slide up while it progresses, applied to the labels local position")]
        public float SlideUpAmount = 1f;
        public LiveScoreInstanceWidgetBehaviour GUIElementPrefab = null;
        public LiveScoreStyleBehaviour DefaultStyle = null;

        public Color PositiveTextColor = Color.white;
        public Color PositiveTextOutline = Color.black;
        public Color NegativeTextColor = Color.white;
        public Color NegativeTextOutline = Color.black;

        #region Public Methods

        public void addScore(Vector3 originInWorld, string content, bool isPositive = true)
        {
            LiveScore score = createLiveScoreInstance(originInWorld, content, isPositive);
            mScores.Add(score);
        }

        public void addScore(Vector3 originInWorld, int points)
        {
            if (points >= 0)
                addScore(originInWorld, "+" + points.ToString(), true);
            else
                addScore(originInWorld, points.ToString(), false);
        }

        public void addScore(Vector3 originInWorld, float points)
        {
            if (points >= 0)
                addScore(originInWorld, "+" + points.ToString("N2"), true);
            else
                addScore(originInWorld, points.ToString("N2"), false);
        }


        public void addScoreInGUI(Vector3 originInGUI, string content, bool isPositive = true)
        {
            LiveScore score = createLiveScoreInstanceInGUI(originInGUI, content, isPositive);
            mScores.Add(score);
        }

        public void addScoreInGUI(Vector3 originInGUI, int points)
        {
            if (points >= 0)
                addScoreInGUI(originInGUI, "+" + points.ToString(), true);
            else
                addScoreInGUI(originInGUI, points.ToString(), false);
        }

        public void addScoreInGUI(Vector3 originInGUI, float points)
        {
            if (points >= 0)
                addScoreInGUI(originInGUI, "+" + points.ToString("N2"), true);
            else
                addScoreInGUI(originInGUI, points.ToString("N2"), false);
        }

        public void removeAllScores()
        {
            foreach (LiveScore score in mScores)
            {
                score.breakUIObjects();
            }
            mScores.Clear();
        }

        public void resetColors()
        {
            if (DefaultStyle != null)
            {
                PositiveTextColor = DefaultStyle.PositiveTextColor;
                PositiveTextOutline = DefaultStyle.PositiveTextOutline;
                NegativeTextColor = DefaultStyle.NegativeTextColor;
                NegativeTextOutline = DefaultStyle.NegativeTextOutline;
            }
        }


        #endregion

        #region Private Types

        /// <summary>
        /// DataHolder for spawend score instances
        /// </summary>
        class LiveScore
        {
            public string Content = "000";
            public bool IsPositive = true;
            /// <summary>
            /// Defines an Origin in World Space
            /// </summary>
            public Vector3 OriginInWorld = Vector3.zero;
            /// <summary>
            /// Defines an Origin in GUI Space
            /// </summary>
            public Vector3 OriginInGUI = Vector3.zero;
            /// <summary>
            /// If true, OriginInWorld is used,
            /// if false, OriginInGUI is used
            /// </summary>
            public bool IsInWorld = true;

            private float mProgress = 0f;
            public float Progress
            {
                get
                {
                    return mProgress;
                }
                set
                {
                    mProgress = Mathf.Clamp01(value);
                }
            }
            public LiveScoreInstanceWidgetBehaviour UIElement = null;

            /// <summary>
            /// If OffLimits is true, this LiveScore would be shown behind the render layer, as the origin is behind the camera
            /// </summary>
            public bool OffLimits = false;
            /// <summary>
            /// When true, shows a pointer that points to the left
            /// </summary>
            public bool PointLeft = false;
            /// <summary>
            /// When true, shows a pointer that points to the right
            /// </summary>
            public bool PointRight = false;
            /// <summary>
            /// When true, shows a pointer that points upwards
            /// </summary>
            public bool PointUp = false;
            /// <summary>
            /// When true, shows a pointer that points downwards
            /// </summary>
            public bool PointDown = false;

            public Color PositiveTextColor = Color.white;
            public Color PositiveTextOutline = Color.black;
            public Color NegativeTextColor = Color.white;
            public Color NegativeTextOutline = Color.black;

            /// <summary>
            /// Updates the UI Element from data in this instance
            /// </summary>
            public void updateUIElement()
            {
                if (UIElement != null)
                {
                    UIElement.gameObject.SetActive(OffLimits);
                    if (UIElement.Label != null)
                    {
                        UIElement.Label.text = Content;
                        UIElement.Label.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.Label.effectColor = (IsPositive) ? PositiveTextOutline : NegativeTextOutline;
                        UIElement.Label.alpha = 1f - Progress;

                        if (PointLeft)
                            UIElement.Label.alignment = NGUIText.Alignment.Left;
                        else if (PointRight)
                            UIElement.Label.alignment = NGUIText.Alignment.Right;
                        else
                            UIElement.Label.alignment = NGUIText.Alignment.Center;
                    }
                    if (UIElement.PointUp != null)
                    {
                        UIElement.PointUp.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointUp.alpha = 1f - Progress;
                        UIElement.PointUp.enabled = PointUp && !PointLeft && !PointRight;
                    }
                    if (UIElement.PointRight != null)
                    {
                        UIElement.PointRight.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointRight.alpha = 1f - Progress;
                        UIElement.PointRight.enabled = PointRight && !PointUp && !PointDown;
                    }
                    if (UIElement.PointDown != null)
                    {
                        UIElement.PointDown.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointDown.alpha = 1f - Progress;
                        UIElement.PointDown.enabled = PointDown && !PointLeft && !PointRight;
                    }
                    if (UIElement.PointLeft != null)
                    {
                        UIElement.PointLeft.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointLeft.alpha = 1f - Progress;
                        UIElement.PointLeft.enabled = PointLeft && !PointUp && !PointDown;
                    }
                    if (UIElement.PointLeftDown != null)
                    {
                        UIElement.PointLeftDown.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointLeftDown.alpha = 1f - Progress;
                        UIElement.PointLeftDown.enabled = PointLeft && PointDown;
                    }
                    if (UIElement.PointLeftUp != null)
                    {
                        UIElement.PointLeftUp.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointLeftUp.alpha = 1f - Progress;
                        UIElement.PointLeftUp.enabled = PointLeft && PointUp;
                    }
                    if (UIElement.PointRightDown != null)
                    {
                        UIElement.PointRightDown.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointRightDown.alpha = 1f - Progress;
                        UIElement.PointRightDown.enabled = PointRight && PointDown;
                    }
                    if (UIElement.PointRightUp != null)
                    {
                        UIElement.PointRightUp.color = (IsPositive) ? PositiveTextColor : NegativeTextColor;
                        UIElement.PointRightUp.alpha = 1f - Progress;
                        UIElement.PointRightUp.enabled = PointRight && PointUp;
                    }
                }
            }

            /// <summary>
            /// Removes all GameObjects and connections of this livescore
            /// (makes it ready to be rmeoved from the internal list)
            /// </summary>
            public void breakUIObjects()
            {
                if (UIElement != null)
                {
                    GameObject.Destroy(UIElement.gameObject);
                    UIElement = null;
                }
            }

            /// <summary>
            /// Resets all pointers to false
            /// </summary>
            public void resetPointers()
            {
                PointLeft = false;
                PointRight = false;
                PointDown = false;
                PointUp = false;
            }
        }

        #endregion

        #region Private Fields

        List<LiveScore> mScores = new List<LiveScore>();

        #endregion


        #region Private Methods

        LiveScore createLiveScoreInstance(Vector3 originInWorld, string content = "000", bool isPositive = true)
        {
            LiveScore score = new LiveScore();

            score.OriginInWorld = originInWorld;
            score.IsPositive = isPositive;
            score.Content = content;
            score.UIElement = createUIElement();

            updateLiveScore(score, true);

            return score;
        }
        LiveScore createLiveScoreInstanceInGUI(Vector3 originInGUI, string content = "000", bool isPositive = true)
        {
            LiveScore score = new LiveScore();

            score.OriginInGUI = originInGUI;
            score.IsInWorld = false;
            score.IsPositive = isPositive;
            score.Content = content;
            score.UIElement = createUIElement();

            updateLiveScore(score, true);

            return score;
        }

        /// <summary>
        /// Creates the UIElement for a LiveScore
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        LiveScoreInstanceWidgetBehaviour createUIElement()
        {
            GameObject uiElement = GameObject.Instantiate(GUIElementPrefab.gameObject) as GameObject;

            Vector3 lScale = uiElement.transform.localScale;
            Vector3 lPos = uiElement.transform.localPosition;
            Quaternion lRot = uiElement.transform.localRotation;

            uiElement.transform.parent = Container.transform;

            uiElement.transform.localScale = lScale;
            uiElement.transform.localPosition = lPos;
            uiElement.transform.localRotation = lRot;

            uiElement.layer = Container.gameObject.layer;
            return uiElement.GetComponent<LiveScoreInstanceWidgetBehaviour>();
        }

        /// <summary>
        /// Removes all score from the internal list that have no UI element
        /// and are therefore marked for deletion
        /// </summary>
        void sweepScores()
        {
            mScores.RemoveAll(score => score.UIElement == null);
        }

        /// <summary>
        /// Converts a world position to a position
        /// that is seen as relative to the container in the GUI
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        Vector3 getScoreGUIPosition(Vector3 worldPosition)
        {
            return Helper.convertScreenSpaceToGUISpace(Camera.main.WorldToScreenPoint(worldPosition));
        }

        /// <summary>
        /// Fits the label into the bounds of the container
        /// </summary>
        /// <param name="nguiPosition"></param>
        /// <returns>True if the labels position had to be clamped/changed</returns>
        bool clampInContainer(UIWidget label)
        {
            Bounds bounds = label.CalculateBounds();
            bounds.center = label.transform.localPosition;
            return Container.ConstrainTargetToBounds(label.transform, ref bounds, true);
        }

        void updateLiveScore(LiveScore score, bool doNotProgress = false)
        {
            //Without an UIElement, this element going to be removed
            if (score.UIElement == null)
                return;
            //Progress was already at maximum, break it so it will be removed
            if (score.Progress == 1f)
            {
                score.breakUIObjects();
                return;
            }


            //Get position of UIElement in GUI space
            Vector3 containerPoint = score.OriginInGUI;
            if(score.IsInWorld)
                containerPoint = getScoreGUIPosition(score.OriginInWorld);
            //Apply calculated GUI position
            score.UIElement.transform.localPosition = containerPoint;
            //Reset the flags,so the label has its original settings & dimensions
            score.resetPointers();

            //Calculate if the point was offLimits
            // According to http://answers.unity3d.com/questions/454457/which-plane-is-which-with-calculatefrustumplanes.html
            Plane nearClipPlane = GeometryUtility.CalculateFrustumPlanes(Camera.main)[4];
            score.OffLimits = nearClipPlane.GetSide(score.OriginInWorld);

            //Check if clamping is needed
            bool clampOriginal = clampInContainer(score.UIElement);
            //If we had to clamp, this means the label is off-container, so we set its flag
            if (clampOriginal)
            {
                Vector3 offset = score.UIElement.transform.localPosition - containerPoint;
                
                float angle = ToolbAR.Math.Vector3Extensions.getSignedAngleTo(
                    new Vector3(1, 1, 0),
                    new Vector3(offset.x, offset.y, 0),
                    new Vector3(0, 0, 1)) + 180f;
                float quadrant = angle / 90f;
                if (quadrant > 3f)
                {
                    score.PointRight = true;
                    if (quadrant >= 3.75f)
                    {
                        score.PointUp = true;
                    }
                    else if (quadrant <= 3.25f)
                    {
                        score.PointDown = true;
                    }
                }
                else if (quadrant > 2f)
                {
                    score.PointDown = true;
                    if (quadrant >= 2.75f)
                    {
                        score.PointRight = true;
                    }
                    else if (quadrant <= 2.25f)
                    {
                        score.PointLeft = true;
                    }
                }
                else if (quadrant > 1f)
                {
                    score.PointLeft = true;
                    if (quadrant >= 1.75f)
                    {
                        score.PointDown = true;
                    }
                    else if (quadrant <= 1.25f)
                    {
                        score.PointUp = true;
                    }
                }
                else
                {
                    score.PointUp = true;
                    if (quadrant >= 0.75f)
                    {
                        score.PointLeft = true;
                    }
                    else if (quadrant <= 0.25f)
                    {
                        score.PointRight = true;
                    }
                }

            }
            if (!doNotProgress)
            {
                //progress
                score.Progress += Time.deltaTime * (1f / LiveDuration);
            }

            //Slide up according to progress
            score.UIElement.transform.localPosition += Vector3.up * (SlideUpAmount * score.Progress);

            //Update colors
            score.PositiveTextColor = PositiveTextColor;
            score.PositiveTextOutline = PositiveTextOutline;
            score.NegativeTextColor = NegativeTextColor;
            score.NegativeTextOutline = NegativeTextOutline;

            score.updateUIElement();

            //Finally clamp to the container anyway
            clampInContainer(score.UIElement);
        }

        #endregion

        #region Unity Messages

        void Update()
        {
            foreach (LiveScore score in mScores)
            {
                updateLiveScore(score);
            }

            sweepScores();

        }

        #endregion
    }
}