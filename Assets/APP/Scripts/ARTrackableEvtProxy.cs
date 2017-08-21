using Polycular;
using UnityEngine;
using Vuforia;

namespace EcoGotchi
{
	public class ARTrackableEvtProxy : MonoBehaviour, ITrackableEventHandler
	{
		TrackableBehaviour m_trackableBehaviour;


		public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
		{
			string imgTargetName;
			var imgTargetBeh = GetComponent<ImageTargetBehaviour>();

			if (imgTargetBeh.ImageTarget != null)
			{
				imgTargetName = imgTargetBeh.ImageTarget.Name;
			}
			else
			{
				imgTargetName = null;
			}

			if (newStatus == TrackableBehaviour.Status.NOT_FOUND)
			{
				// Debug.Log("Tracking state NOT_FOUND: " + imgTargetName);
				Eventbus.Instance.FireEvent<ARTrackableStateChangedEvent>(new ARTrackableStateChangedEvent(false, imgTargetName));
			}
			else if (newStatus == TrackableBehaviour.Status.TRACKED)
			{
				// Debug.Log("Tracking state TRACKED: " + imgTargetName);
				Eventbus.Instance.FireEvent<ARTrackableStateChangedEvent>(new ARTrackableStateChangedEvent(true, imgTargetName));
			}
		}

		void Start()
		{
			m_trackableBehaviour = GetComponent<TrackableBehaviour>();
			if (m_trackableBehaviour)
			{
				m_trackableBehaviour.RegisterTrackableEventHandler(this);
			}
		}
	}
}