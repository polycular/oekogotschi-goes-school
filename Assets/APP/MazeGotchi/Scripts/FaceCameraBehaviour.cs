using UnityEngine;

namespace MazeGotchi
{
	public class FaceCameraBehaviour : MonoBehaviour
	{
		bool mFirstUpdateDone = false;
		bool mSpriteEnabled = false;


		void Update()
		{
			if (mFirstUpdateDone && !mSpriteEnabled)
			{
				GetComponentInChildren<SpriteRenderer>().enabled = true;
				mSpriteEnabled = true;
			}

			transform.LookAt(Camera.main.transform.position, transform.up);
			mFirstUpdateDone = true;
		}
	}
}