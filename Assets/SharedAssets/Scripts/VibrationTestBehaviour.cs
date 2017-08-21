using UnityEngine;
using System.Collections;
using SharedAssets;

namespace FishingGotchi
{
    [RequireComponent(typeof(VibrationBehaviour))]
    public class VibrationTestBehaviour : MonoBehaviour
    {
        public GameObject TestObject = null;
        public VibrationBehaviour VibrationBehaviour = null;

        // Use this for initialization
        void Start()
        {
            if (VibrationBehaviour == null)
                VibrationBehaviour = GetComponent<VibrationBehaviour>();
        }

        // Update is called once per frame
        void Update()
        {
            checkOnClick();
        }

        private void checkOnClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == TestObject)
                    {
                        VibrationBehaviour.vibrate();
                    }
                }
            }
        }
    }
}
