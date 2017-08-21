using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SharedAssets.GameManagement
{
    public class MiniGameListBehaviour : MonoBehaviour, IEnumerable
    {

        Dictionary<string, MiniGameBehaviour> mMiniGameBehaviours = null;

        public Dictionary<string, MiniGameBehaviour> MiniGameBehaviours
        {
            get
            {
                return mMiniGameBehaviours;
            }
        }

        public MiniGameBehaviour this[string name]
        {
            get {
                MiniGameBehaviour result;
                mMiniGameBehaviours.TryGetValue(name, out result);
                return result;
            }
        }

        void loadMiniGames()
        {
            mMiniGameBehaviours = new Dictionary<string, MiniGameBehaviour>();
            foreach (MiniGameBehaviour minigamePrefab in Resources.LoadAll<MiniGameBehaviour>("Prefabs/MiniGames/"))
            {
                GameObject minigame = GameObject.Instantiate(minigamePrefab.gameObject) as GameObject;
                minigame.name = minigamePrefab.gameObject.name;
                minigame.transform.parent = gameObject.transform;
                MiniGameBehaviour minigameB = minigame.GetComponent<MiniGameBehaviour>();
                mMiniGameBehaviours.Add(minigame.name, minigameB);
            }
        }

        void Awake()
        {
            loadMiniGames();
        }

        public IEnumerator GetEnumerator()
        {
            return mMiniGameBehaviours.GetEnumerator();
        }
    }
}