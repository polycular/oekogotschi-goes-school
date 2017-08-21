using UnityEngine;

namespace EcoGotchi
{
	public class FingerTrail : MonoBehaviour
	{
		TrailRenderer m_trail;

		void Start()
		{
			m_trail = GetComponent<TrailRenderer>();
		}

		void OnDisable()
		{
			m_trail.Clear();
		}

		void Update()
		{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

			if (Input.touchCount >= 1)
			{
				var input = Input.GetTouch(0);

				if (input.phase == TouchPhase.Moved)
				{
					UpdatePosition(input.position);
				}
			}
#elif UNITY_EDITOR

			if (Input.GetMouseButton(0))
			{
				UpdatePosition(Input.mousePosition);
			}
#endif
		}

		void UpdatePosition(Vector2 newPos)
		{
			var pos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, 1f));
			gameObject.transform.position = pos;
		}
	}

}
