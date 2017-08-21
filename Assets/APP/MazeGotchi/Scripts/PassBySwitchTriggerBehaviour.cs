using UnityEngine;
using System.Collections;
using ToolbAR;

namespace MazeGotchi
{
    /************************************************************************/
    /* Populates event when a gameobject passes by the switch trigger.      */
    /************************************************************************/
    public class PassBySwitchTriggerBehaviour : MonoBehaviour
    {
        public delegate void OnPassBySwitchTriggerHandler(MazeSwitchBehaviour mazeSwitch);
        public event OnPassBySwitchTriggerHandler OnPassBySwitchTrigger;
        public MazeSwitchBehaviour MazeSwitch;
        public float MinTimeBetweenTriggerInvokes = 0.5f;
        private float m_TimeSinceLastTriggerInvoke = 0.0f;

        // Update is called once per frame
        void Update()
        {
            m_TimeSinceLastTriggerInvoke += Time.deltaTime;
        }

        void OnTriggerExit(Collider other)
        {
            //only detect trigger invokes after a certain time since the last invoke has passed and the velocity of the passing object points downwards

            if (m_TimeSinceLastTriggerInvoke >= MinTimeBetweenTriggerInvokes && other.gameObject.GetComponent<Rigidbody>().velocity.y < 0)
            {
                
                if (OnPassBySwitchTrigger != null)
                {

                    OnPassBySwitchTrigger(MazeSwitch);
                }

                m_TimeSinceLastTriggerInvoke = 0.0f;
            }

        }
    }

}
