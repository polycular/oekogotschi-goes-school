using UnityEngine;
using System.Collections;

namespace MazeGotchi
{
    /************************************************************************/
    /* Defines a switch with two possible rotations, between which can be switched.*/
    /************************************************************************/
    public class MazeSwitchBehaviour : MonoBehaviour {
        public int m_GroupID = 0;
        private enum Rotation { POS1 = 45, POS2 = 315 };
        private int currPos = (int) Rotation.POS1;

	    // Use this for initialization
	    void Start () {
	        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(gameObject.transform.localRotation.x, (int) Rotation.POS1, gameObject.transform.localRotation.z));
	    }

        public void setGroupID(int groupID)
        {
            m_GroupID = groupID;
        }
        public int getGroupID()
        {
            return m_GroupID;
        }

        public void triggerSwitch()
        {
            currPos = (currPos == (int)Rotation.POS1) ? (int)Rotation.POS2 : (int)Rotation.POS1;
            gameObject.transform.localRotation = Quaternion.Euler(new Vector3(gameObject.transform.localRotation.x, currPos, gameObject.transform.localRotation.z));  
        }

    }
}
