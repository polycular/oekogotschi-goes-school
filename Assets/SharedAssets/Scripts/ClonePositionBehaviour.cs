using UnityEngine;
using System.Collections;

namespace SharedAssets
{
    [ExecuteInEditMode]
    public class ClonePositionBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool InEditor = false;
#endif
        //The Source from which to clone
        public Transform CloneSource = null;
        /**
         * If a Pivot is given, the cloned attribute will be matched against the pivot,
         * and this object is moved in a way so the pivot would match the clonesource in this objects transform space.
         * 
         * Use case: Pick a child object of this object, so -this- object always moves to the trackable, not with the center but with the pivot
         * */
        // 
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
                if (!CloneSource.position.Equals(useTransform.position))
                {
                    if (!OnlyWhenSourceIsActive || CloneSource.gameObject.activeInHierarchy)
                    {
                        Vector3 diff = CloneSource.position - useTransform.position;
                        transform.position += diff;
                    }
                }
            }
        }
    }
}