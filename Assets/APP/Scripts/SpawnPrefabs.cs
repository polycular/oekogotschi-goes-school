using UnityEngine;
using System.Collections.Generic;

namespace EcoGotchi
{
	public class SpawnPrefabs : MonoBehaviour
	{
		public List<GameObject> PrefabsToSpawn;

		void Start()
		{
			foreach (var go in PrefabsToSpawn)
			{
				Instantiate(go, transform);
			}
		}
	}
}