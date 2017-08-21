using UnityEngine;
using System.Collections;

namespace MazeGotchi
{
    public class TrashItemBehaviour : MonoBehaviour
    {
        public BinManagerBehaviour.BinType TrashType;
        public string ItemDescription;
        public float SelfDestroyTimeThreshold = 5.2f;
        private float mTimeFalling = 0;

        public delegate void OnSelfDestroyHandler(TrashItemBehaviour item);
        public event OnSelfDestroyHandler onSelfDestroy;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!gameObject.GetComponent<Rigidbody>().isKinematic)
            {
                mTimeFalling += Time.deltaTime;

                if (mTimeFalling >= SelfDestroyTimeThreshold)
                {
                    if (onSelfDestroy != null)
                        onSelfDestroy(this);

                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}