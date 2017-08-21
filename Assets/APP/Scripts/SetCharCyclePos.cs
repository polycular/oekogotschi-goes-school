using UnityEngine;

public class SetCharCyclePos : MonoBehaviour
{
	void Start()
	{
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.position = new Vector3(1f, 7.5f, 0.7f);
	}
}
