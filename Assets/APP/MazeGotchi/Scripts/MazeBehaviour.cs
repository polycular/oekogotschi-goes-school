using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGotchi
{
	/************************************************************************/
	/*  Defines a maze with switches (that may belong to groups),           */
	/*  which move when something passes by.                                */
	/*  (Listening to OnPassBySwitchTriggerHandler event)                   */
	/************************************************************************/
	public class MazeBehaviour : MonoBehaviour
    {

        public Material[] m_GroupMaterials;
        //This is the time the doors need to open one way. The complete door moving time is this time * 3. 
        //(it keeps open for the same timeframe and needs the same timeframe to close again)
        public float TrapDoorOpenTimeInSeconds = 0.3f;
        public Transform TrapDoorLeft, TrapDoorRight;

        public delegate void AfterTrapDoorClosedHandler();
        public event AfterTrapDoorClosedHandler afterTrapDoorClosed;

        private float mTrapDoorClosedPosition = 0.0f, mTrapDoorOpenedRotation = 60.0f;
        private List<MazeSwitchBehaviour>[] m_SwitchGroups = new List<MazeSwitchBehaviour>[10];
        private bool mIsTrapDoorOpen = false;

        void Start()
        {
            init();
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
            if (mIsTrapDoorOpen)
            {

                TrapDoorLeft.transform.localEulerAngles = new Vector3(0, 0, mTrapDoorClosedPosition);
                TrapDoorRight.transform.localEulerAngles = new Vector3(0, 0, -mTrapDoorClosedPosition);
                mIsTrapDoorOpen = false;
                if (afterTrapDoorClosed != null)
                    afterTrapDoorClosed();
            }
        }

        public void init()
        {
            for (int i = 0; i < m_SwitchGroups.Length; i++)
                m_SwitchGroups[i] = new List<MazeSwitchBehaviour>();

            MazeSwitchBehaviour[] mazeswitches = GetComponentsInChildren<MazeSwitchBehaviour>(true);
            foreach (MazeSwitchBehaviour mazeswitch in mazeswitches)
            {
                m_SwitchGroups[mazeswitch.getGroupID()].Add(mazeswitch);

                mazeswitch.gameObject.GetComponent<Renderer>().material = m_GroupMaterials[mazeswitch.getGroupID()];
            }

            PassBySwitchTriggerBehaviour[] triggers = GetComponentsInChildren<PassBySwitchTriggerBehaviour>(true);
            foreach (PassBySwitchTriggerBehaviour pbstb in triggers)
            {
                pbstb.OnPassBySwitchTrigger += new PassBySwitchTriggerBehaviour.OnPassBySwitchTriggerHandler(onPassBySwitchTrigger);
            }
        }

        void onPassBySwitchTrigger(MazeSwitchBehaviour mazeSwitch)
        {
            triggerSwitchGroup(mazeSwitch.getGroupID());
        }

        public List<MazeSwitchBehaviour>[] getSwitchGroups()
        {
            return m_SwitchGroups;
        }

        public void triggerSwitchGroup(int groupID)
        {
            List<MazeSwitchBehaviour> groupSwitches = m_SwitchGroups[groupID];
            for (int i = 0; i < groupSwitches.Count; i++)
            {
                MazeSwitchBehaviour mzb = groupSwitches[i].GetComponent(typeof(MazeSwitchBehaviour)) as MazeSwitchBehaviour;
                if (mzb != null)
                {
                    mzb.triggerSwitch();
                }
            }
        }

        public Material[] getGroupMaterials()
        {
            return m_GroupMaterials;
        }

        public bool openTrapDoor()
        {
            if (mIsTrapDoorOpen)
            {
                return false;
            }
            else
            {
                mIsTrapDoorOpen = true;
                StartCoroutine("moveTrapDoor");
                return true;
            }
        }

        IEnumerator moveTrapDoor()
        {
            float elapsedTime = 0.0f;
            float keepOpenTimeInSeconds = 1.0f;

			var tdLeftRot = TrapDoorLeft.transform.localRotation;
			var tdRightRot = TrapDoorRight.transform.localRotation;

            float lerp_tvalue = 0;

            //open the doors
            while (elapsedTime <= TrapDoorOpenTimeInSeconds)
            {
                lerp_tvalue = elapsedTime / TrapDoorOpenTimeInSeconds;
                float lerpedRotation = Mathf.Lerp(mTrapDoorClosedPosition, mTrapDoorOpenedRotation, lerp_tvalue);
                TrapDoorLeft.transform.localEulerAngles = new Vector3(0, 0, lerpedRotation);
                TrapDoorRight.transform.localEulerAngles = new Vector3(0, 0, -lerpedRotation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            elapsedTime = 0;
            lerp_tvalue = 0;

            //keep doors open
            yield return new WaitForSeconds(keepOpenTimeInSeconds);

            //close doors
            while (elapsedTime <= TrapDoorOpenTimeInSeconds)
            {
                lerp_tvalue = elapsedTime / TrapDoorOpenTimeInSeconds;
                lerp_tvalue = 1 - lerp_tvalue;
                float lerpedRotation = Mathf.Lerp(mTrapDoorClosedPosition, mTrapDoorOpenedRotation, lerp_tvalue);
                TrapDoorLeft.transform.localEulerAngles = new Vector3(0, 0, lerpedRotation);
                TrapDoorRight.transform.localEulerAngles = new Vector3(0, 0, -lerpedRotation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

			TrapDoorLeft.transform.localRotation = tdLeftRot;
			TrapDoorRight.transform.localRotation = tdRightRot;

            mIsTrapDoorOpen = false;

            if (afterTrapDoorClosed != null)
                afterTrapDoorClosed();
        }
    }

}
