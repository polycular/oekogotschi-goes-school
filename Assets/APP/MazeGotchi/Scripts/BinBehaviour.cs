using UnityEngine;
using System.Collections;

namespace MazeGotchi
{
    public class BinBehaviour : MonoBehaviour
    {
        public BinManagerBehaviour.BinType Type;
        public delegate void OnCorrectItemCatchedHandler(TrashItemBehaviour item);
        public delegate void OnIncorrectItemCatchedHandler(BinBehaviour bin, TrashItemBehaviour item);
        public event OnCorrectItemCatchedHandler onCorrectItemCatched;
        public event OnIncorrectItemCatchedHandler onIncorrectItemCatched;
        public bool ShowDebugLogs = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public BinManagerBehaviour.BinType getBinType()
        {
            return Type;
        }

        void OnTriggerEnter(Collider other)
        {
            //check if catched item was of the correct trash type
            TrashItemBehaviour item = other.GetComponent<TrashItemBehaviour>();
            if (item != null)
            {
                if (item.TrashType == Type)
                {
                    //a correct item has been catched

                    if (ShowDebugLogs)
                        ToolbAR.LogAR.log("An item has been catched with the correct bin", this, this);

                    if (onCorrectItemCatched != null)
                    {
                        onCorrectItemCatched(item);
                    }
                }
                else
                {
                    //the catched item is of a different type than the bin

                    if (ShowDebugLogs)
                        ToolbAR.LogAR.log("An item has been catched with an incorrect bin", this, this);

                    if (onIncorrectItemCatched != null)
                    {
                        onIncorrectItemCatched(this, item);
                    }

                }
                Destroy(other.gameObject);
            }
        }
    }
}
