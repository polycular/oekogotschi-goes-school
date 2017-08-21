using UnityEngine;
using System.Collections;

namespace Inventory
{
    public class ItemBehaviour : MonoBehaviour
    {
        public string ItemName;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public T hasScript<T>() where T : MonoBehaviour
        {
            return gameObject.GetComponent<T>();
        }

        public bool getScript<T>() where T : MonoBehaviour
        {
            if(hasScript<T>() != null)
                return true;
            return false;
        }
    }
}