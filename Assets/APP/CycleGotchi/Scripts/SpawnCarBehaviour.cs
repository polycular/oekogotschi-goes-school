using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CycleGotchi
{
    public class SpawnCarBehaviour : MonoBehaviour
    {
        public CarBehaviour CarPrefab;
        public bool UseRandomProgress = true;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Spawns the given amount of cars onto the streetgrid, randomly selecting tiles of the grid as start tile.
        /// A tile can only be starting tile for one car, therefore the maximum amount of cars is the amount of tiles in the streetgrid. 
        /// </summary>
        /// <param name="amount">[0 .. Streetgrid Tilecount] The Amount is limited by the tilecount of the streetgrid, as there can only spawn one car per tile.</param>
        /// <param name="streetGrid">The streetgrid the cars should spawn on.</param>
        /// <param name="useRandomOnTileStartPositions">False, if the cars should always start driving at the start position of the tile. 
        /// True, if the car starts with a random progression value, meaning a car could for example start driving in the middle of a tile.</param>
        /// <returns></returns>
        public List<CarBehaviour> spawnCars(int amount, StreetGridBehaviour streetGrid)
        {
            List<CarBehaviour> spawnedCars = new List<CarBehaviour>();
            List<TileBehaviour> possibleSpawnTiles = new List<TileBehaviour>();
            foreach (TileBehaviour tile in streetGrid.Tiles)
            {
                possibleSpawnTiles.Add(tile);
            }

            for (int i = 0; i < amount; i++)
            {
                GameObject car = GameObject.Instantiate(CarPrefab.gameObject) as GameObject;
                CarBehaviour carB = car.GetComponent<CarBehaviour>();

                Vector3 carScale = car.transform.localScale;
                Quaternion carRotation = car.transform.localRotation;

                car.transform.parent = this.transform;
                car.transform.localScale = carScale;
                car.transform.localRotation = carRotation;

                int tileIndex = Random.Range(0, possibleSpawnTiles.Count);
                int edge = Random.Range(0, 5);
                carB.initialize(possibleSpawnTiles[tileIndex], (TileBehaviour.Edge)edge);

                possibleSpawnTiles.RemoveAt(tileIndex);

                spawnedCars.Add(carB);

                if (UseRandomProgress)
                {
                    carB.setProgressOnTile(Random.Range(0.0f, 1.0f));
                }
            }

            return spawnedCars;
        }
    }
}