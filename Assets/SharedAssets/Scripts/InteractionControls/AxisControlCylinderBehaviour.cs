using UnityEngine;
using System.Collections;
using ToolbAR.Math;
namespace SharedAssets
{
    namespace InteractionControls
    {
        //only meant to be used in combination on a plain cylinder object with a convex cylinder collider
        public class AxisControlCylinderBehaviour : MonoBehaviour
        {
            public bool ReactOnLateralSurface = true;
            public bool ReactOnFloor = true;
            public bool ReactOnCap = true;

            public float DegreeStepsRadial = 0f;
            public float DegreeStepsLinear = 0f;

            //Returns the angle that is defined by this AxisCylinder
            public float AngleAxis
            {
                get
                {
                    return transform.localRotation.eulerAngles.y;
                }
            }

            public enum InteractionMode
            {
                NONE,
                LINEAR_LR, //Linear from left to right
                ANGLE_TOP, //For top side, the firts select pivot point is used to calculate an angle
                ANGLE_BOTTOM //For bottom side, the firts select pivot point is used to calculate an angle
            }

            InteractionMode mCurrentMode = InteractionMode.NONE;
            Vector3 mPivotPos = Vector3.zero;
            Quaternion mOriginalRotation = Quaternion.identity;

            public InteractionMode CurrentMode
            {
                get
                {
                    return mCurrentMode;
                }
            }
            public bool IsInteracting
            {
                get
                {
                    return mCurrentMode != InteractionMode.NONE;
                }
            }

            //Cancels current interaction and resets rotation
            public void cancel()
            {
                transform.rotation = mOriginalRotation;
                mCurrentMode = InteractionMode.NONE;
            }

            Vector3 getTopCenterInScreenCoordinates(Camera cam, bool botomInsteadOfTop)
            {
                Vector3 centerPos = transform.position;
                if (botomInsteadOfTop)
                {
                    centerPos -= Vector3.up * transform.lossyScale.y;
                }
                else
                {
                    centerPos += Vector3.up * transform.lossyScale.y;
                }
                return cam.WorldToScreenPoint(centerPos);
            }

            void updateByLinear()
            {
                Vector3 axisOnScreen =
                    getTopCenterInScreenCoordinates(Camera.main, false)
                    -
                    getTopCenterInScreenCoordinates(Camera.main, true)
                    ;
                axisOnScreen.z = 0;
                axisOnScreen.Normalize();
                float transformAngle = Vector3.up.getSignedAngleTo(axisOnScreen, Vector3.back);

                Vector3 curPos = Quaternion.Euler(0, 0, transformAngle) * Input.mousePosition;
                Vector3 pivotPos = Quaternion.Euler(0, 0, transformAngle) * mPivotPos;
                
                float anglePct = (curPos.x - pivotPos.x) / Screen.width;
                float angle = anglePct * -360f;

                if (DegreeStepsLinear > 0f)
                    angle = DegreeStepsLinear * Mathf.Round(angle / DegreeStepsLinear);

                transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
            }

            void updateByAngle()
            {
                Vector3 curPos = Input.mousePosition;
                Vector3 centerPos = getTopCenterInScreenCoordinates(Camera.main, mCurrentMode == InteractionMode.ANGLE_BOTTOM);
                float angle = (mPivotPos - centerPos).getSignedAngleTo(curPos - centerPos, Vector3.back);
                if (mCurrentMode == InteractionMode.ANGLE_BOTTOM)
                    angle *= -1;


                if (DegreeStepsRadial > 0f)
                    angle = DegreeStepsRadial * Mathf.Round(angle / DegreeStepsRadial);

                transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
            }

            void Update()
            {
                if (IsInteracting)
                {
                    if (!Input.GetMouseButton(0))
                    {
                        //Interaction is done
                        mCurrentMode = InteractionMode.NONE;
                    }
                    else
                    {
                        switch (mCurrentMode)
                        {
                            case InteractionMode.LINEAR_LR:
                                updateByLinear();
                                break;
                            case InteractionMode.ANGLE_TOP:
                            case InteractionMode.ANGLE_BOTTOM:
                                updateByAngle();
                                break;
                        }
                    }
                }
            }

            void OnMouseDown()
            {
                if (Camera.main != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (GetComponent<Collider>().Raycast(ray, out hit, 100f))
                    {
                        if (hit.normal == transform.up)
                        {
                            if (ReactOnCap)
                            {
                                mCurrentMode = InteractionMode.ANGLE_TOP;
                                mPivotPos = Input.mousePosition;
                                mOriginalRotation = transform.localRotation;

                            }
                        }
                        else if (hit.normal == transform.up * -1f)
                        {
                            if (ReactOnFloor)
                            {
                                mCurrentMode = InteractionMode.ANGLE_BOTTOM;
                                mPivotPos = Input.mousePosition;
                                mOriginalRotation = transform.localRotation;
                            }
                        }
                        else
                        {
                            if (ReactOnLateralSurface)
                            {
                                mCurrentMode = InteractionMode.LINEAR_LR;
                                mPivotPos = Input.mousePosition;
                                mOriginalRotation = transform.localRotation;
                            }
                        }
                    }
                }
            }
        }
    }
}