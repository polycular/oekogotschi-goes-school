using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Inventory
{
    public class InventoryBehaviour : MonoBehaviour
    {
        public List<ItemBehaviour> Items = new List<ItemBehaviour>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void addItem(ItemBehaviour item)
        {
            Items.Add(item);
        }

        public bool removeItem(ItemBehaviour item)
        {
            return Items.Remove(item);
        }

        public void clearInventory()
        {
            Items.Clear();
        }

        public List<ItemBehaviour> getItems()
        {
            return Items;
        }

        public List<T> getItemsWithBehaviour<T>() where T : MonoBehaviour
        {
            List<T> result = new List<T>();
 
            foreach(ItemBehaviour i in Items)
            {
                T behaviourScript = i.gameObject.GetComponent<T>();
                if (behaviourScript != null)
                {
                    result.Add(behaviourScript);
                }
            }
            return result;
        }


    }
}
