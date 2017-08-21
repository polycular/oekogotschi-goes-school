using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MazeGotchi
{
    public class GuiInputHandlerBehaviour : MonoBehaviour
    {
        bool mIsActive = true;
        public GameObject CustomGUI;
        [HideInInspector]
        public List<GameObject> TrapDoorTriggers;

        public delegate void OnLeftHandClickHandler();
        public event OnLeftHandClickHandler onLeftHandClick;
        public delegate void OnRightHandClickHandler();
        public event OnRightHandClickHandler onRightHandClick;
        public delegate void OnTrapDoorTriggerHandler();
        public event OnTrapDoorTriggerHandler onTrapDoorTrigger;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (TrapDoorTriggers.Contains (hit.collider.transform.parent.gameObject) || TrapDoorTriggers.Contains (hit.collider.transform.gameObject) )
                    {
                        onTrapDoorTriggerClick();
                    }
                }
            }
        }

        public void onLeftHandButtonClick()
        {
            if (mIsActive)
            {
                if (onLeftHandClick != null)
                    onLeftHandClick();
            }
        }

        public void onRightHandButtonClick()
        {
            if (mIsActive)
            {
                if (onRightHandClick != null)
                    onRightHandClick();
            }
        }

        public void onTrapDoorTriggerClick()
        {
            if (mIsActive)
            {
                if (onTrapDoorTrigger != null)
                    onTrapDoorTrigger();

            }
        }

        public void setActive(bool active)
        {
            if (active != mIsActive)
            {
                mIsActive = active;
                CustomGUI.SetActive(mIsActive);
            }
        }
    }
}