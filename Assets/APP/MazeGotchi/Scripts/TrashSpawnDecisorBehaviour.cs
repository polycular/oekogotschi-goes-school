using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MazeGotchi
{
    /// <summary>
    /// This class represents the Spawn Decisor from EC-81
    /// It generates random sorted selections (bags) form SpawnableItems,
    /// from which the next item will be picked. This ensures that no item gets
    /// pickd too often in succesion and that alle items occur (Depending on the BagSize)
    /// </summary>
    public class TrashSpawnDecisorBehaviour : MonoBehaviour
    {
        public List<TrashItemBehaviour> SpawnableItems = new List<TrashItemBehaviour>();
        [Tooltip("If 0, all SpawnableItems will be present once in a bag.")]
        public int BagSize = 0;

        private Queue<TrashItemBehaviour> mCurrentBag = new Queue<TrashItemBehaviour>();

        public static TrashItemBehaviour[] generateBag(int bagSize, List<TrashItemBehaviour> spawnableItems)
        {
            if (bagSize <= 0 || bagSize > spawnableItems.Count)
            {
                bagSize = spawnableItems.Count;
            }

            TrashItemBehaviour[] bag = new TrashItemBehaviour[bagSize];
            List<int> chosenItems = new List<int>();

            int infiniteLoopBreaker = 150;

            //Fill with null by default
            for (int i = 0; i < bagSize; i++)
            {
                bag[i] = null;
            }

            for(int i = 0; i < bagSize; i++)
            {

                int pick = Random.Range(0, spawnableItems.Count);
                if(!chosenItems.Contains(pick))
                {
                    chosenItems.Add(pick);
                    bag[i] = spawnableItems[pick];
                }
                else
                {
                    //Retry
                    infiniteLoopBreaker--;

                    if (infiniteLoopBreaker == 0)
                    {
                        Debug.LogWarning("TrashSpawnDecisorBehaviour::generateBag seems to be stuck in loop, breaking..");
                        break;
                    }
                    i--;
                }
            }

            return bag;

        }

        /// <summary>
        /// Removes and returns the next item from the bag
        /// </summary>
        /// <returns>A randomly selected Trash Item</returns>
        public TrashItemBehaviour takeNext()
        {
            ensureFilledBag();
            return mCurrentBag.Dequeue();
        }

        /// <summary>
        /// Removes and returns the next item from the bag
        /// </summary>
        /// <returns>A randomly selected Trash Item</returns>
        public TrashItemBehaviour getNext()
        {
            return takeNext();
        }

        /// <summary>
        /// Returns the next item from the bag
        /// </summary>
        /// <returns>A randomly selected Trash Item</returns>
        public TrashItemBehaviour peek()
        {
            ensureFilledBag();
            return mCurrentBag.Dequeue();
        }

        /// <summary>
        /// Checks if the current bag is empty, and fills it again
        /// It is not required to call this form outside, as peek() and getNext() automatically call this
        /// </summary>
        public void ensureFilledBag()
        {
            if (mCurrentBag == null || mCurrentBag.Count == 0)
            {
                mCurrentBag = new Queue<TrashItemBehaviour>(generateBag(BagSize, SpawnableItems));
            }
        }

        /// <summary>
        /// Called in Unity Editor when values on this Behaviour have changed
        /// </summary>
        void OnValidate()
        {
            if (BagSize < 0)
                BagSize = 0;
            if (BagSize > SpawnableItems.Count)
                BagSize = SpawnableItems.Count;
        }
    }
}