using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MintiBlinkBehaviour : MonoBehaviour
{
    public Transform LeftEye;
    public Transform RightEye;
    public float BlinkTimer = 3.0f;
    public float ClosedEyeTime = 0.5f;
    private bool mHasClosedEyes = false;

    void OnEnable()
    {
        StartCoroutine(Blink());
    }

    void OnDisable()
    {
        if (mHasClosedEyes)
        {
            LeftEye.Rotate(-80.0f, 0, 0);
            RightEye.Rotate(-80.0f, 0, 0);
            mHasClosedEyes = false;
        }
    }


    IEnumerator Blink()
    {
        LeftEye.Rotate(80.0f, 0, 0);
        RightEye.Rotate(80.0f, 0, 0);
        mHasClosedEyes = true;
        yield return new WaitForSeconds(ClosedEyeTime);
        LeftEye.Rotate(-80.0f, 0, 0);
        RightEye.Rotate(-80.0f, 0, 0);
        mHasClosedEyes = false;
        yield return new WaitForSeconds(BlinkTimer + Random.value * 2.0f);
        StartCoroutine(Blink());
    }
}
