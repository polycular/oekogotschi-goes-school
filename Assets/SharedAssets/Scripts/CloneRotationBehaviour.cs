using UnityEngine;
using System.Collections;

namespace SharedAssets
{
    [ExecuteInEditMode]
    public class CloneRotationBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool InEditor = false;
#endif
        //The Source from which to clone
        public Transform CloneSource = null;

        public Transform Pivot = null;

        public bool OnlyWhenSourceIsActive = true;

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (!InEditor && !Application.isPlaying)
            {
                return;
            }
#endif

            if (CloneSource != null)
            {
                Transform useTransform = (Pivot != null) ? Pivot.transform : this.transform;
                float angularDiff = Quaternion.Angle(CloneSource.rotation, useTransform.rotation);
                if (angularDiff > Mathf.Epsilon)
                {
                    if (!OnlyWhenSourceIsActive || CloneSource.gameObject.activeInHierarchy)
                    {
                        Quaternion diff = Quaternion.Inverse(useTransform.rotation) * CloneSource.rotation;
                        this.transform.rotation *= diff;
                    }
                }
            }
        }
    }
}