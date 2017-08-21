using UnityEngine;
using Vuforia;

public class ImageTargetTracker : MonoBehaviour , ITrackableEventHandler {

    public delegate void TrackableStateChangeHandler(ImageTargetTracker agent);
    public event TrackableStateChangeHandler onTrackableStateChange;

    private ImageTargetBehaviour mTrackableBehavior;

    public bool IsTracked = false;


    void Start()
    {
        mTrackableBehavior = GetComponent<ImageTargetBehaviour>();
        if (mTrackableBehavior)
			mTrackableBehavior.RegisterTrackableEventHandler(this);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }

        if (onTrackableStateChange != null)
            onTrackableStateChange(this);
    }

    private void OnTrackingFound()
    {
        IsTracked = true;
    }

    private void OnTrackingLost()
    {
        IsTracked = false;
    }
}
