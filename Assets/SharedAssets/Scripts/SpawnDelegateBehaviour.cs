using UnityEngine;

namespace SharedAssets
{
    /// <summary>
    /// Spawns another GameObject and deletes its own GameObject
    /// </summary>
    public class SpawnDelegateBehaviour : MonoBehaviour
    {
        public GameObject PrefabToSpawn;

        void Awake()
        {
            GameObject spawned = Instantiate(PrefabToSpawn, transform) as GameObject;
            if (spawned != null)
            {
                spawned.transform.parent = transform.parent;
            }
            Destroy(gameObject);
        }
    }
}


