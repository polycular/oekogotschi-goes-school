using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public Transform EyeLidTop;
    public Transform EyeLidBottom;

    [Range(0,1)]
    [Tooltip("Chance to Blink in Percentage/WaitForEndOfFrame")]
    public float ChanceToBlink;

    public float BlinkSpeed = 0.4f;

    [Range(0, 1)]
    [Tooltip("0 = Closed, 1 = Open")]
    public float EyeState = 1;

    public float openTop;
    public float openBottom;
    
    bool isBlinking = false;

	void OnDisable()
	{
		EyeState = 1;
		openTop = - 51f;
		openBottom = 51f;

		EyeLidTop.transform.localEulerAngles = new Vector3(openTop * EyeState, 0, 0);
		EyeLidBottom.transform.localEulerAngles = new Vector3(openBottom * EyeState, 0, 0);

		isBlinking = false;
	}

	void FixedUpdate()
	{
	    if (Random.Range(0.0f, 1.0f) < ChanceToBlink)
        {
            if (!isBlinking)
            {
                isBlinking = true;
                StartCoroutine(CloseEye());
            }
        }

        EyeLidTop.transform.localEulerAngles = new Vector3(openTop * EyeState, 0, 0);
        EyeLidBottom.transform.localEulerAngles = new Vector3(openBottom * EyeState, 0, 0);
    }

    IEnumerator CloseEye()
    {
        while (true)
        {
            EyeState = Mathf.Lerp(EyeState, 0, BlinkSpeed);      
			  
            if (EyeState <= 0.05)
            {               
                StartCoroutine(OpenEye());
                yield break;   
            }   
			    
            yield return null;
        }
    }

    IEnumerator OpenEye()
    {
        while(true)
        {
            EyeState = Mathf.Lerp(EyeState, 1, BlinkSpeed);

            if (EyeState >= 0.95)
            {
                StopCoroutine(OpenEye());
                isBlinking = false;
                yield break;
            }

            yield return null;
        }
    }
    
}
