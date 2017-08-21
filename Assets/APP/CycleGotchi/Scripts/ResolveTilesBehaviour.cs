using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace CycleGotchi
{
    /// <summary>
    /// This class handles the tile transformations from bad to good tiles
    /// </summary>
    public class ResolveTilesBehaviour : MonoBehaviour
    {
        public List<TileBehaviour> DebugTiles = new List<TileBehaviour>();
        public GameObject CarContainer = null;

        public GameDirector GameDirector;

        //shall there be debug output in the console for this behaviour?
        public bool PrintDebug = false;

        //fake button: resolve the DebugTiles for testing purposes
        public bool DebugResolveTiles = false;

        #region Unity Messages

        void Start()
        {
            //if not set, this behaviour is attached to the car container directly
            if (CarContainer == null)
                CarContainer = this.gameObject;

            if (GameDirector == null)
                GameDirector = this.GetComponent<GameDirector>();
        }

        void Update()
        {
            if (DebugResolveTiles)
            {
                DebugResolveTiles = false;

                resolveGreenArea(DebugTiles);
            }
        }

        #endregion


        //compatibility: arrays instead of generic lists
        public CarBehaviour[] resolveGreenArea(TileBehaviour[] tiles)
        {
            return resolveGreenArea(new List<TileBehaviour>(tiles)).ToArray();
        }

        /// <summary>
        /// Resolves a given list of tiles by turning them green, removing the cars on them and returning the car behaviours
        /// </summary>
        /// <param name="tiles">list of tiles to resolve</param>
        /// <returns></returns>
        public List<CarBehaviour> resolveGreenArea(List<TileBehaviour> tiles)
        {
            List<CarBehaviour> carsOnTiles = getCarsOnTiles(tiles);

            foreach (TileBehaviour tile in tiles)
            {
                tile.turnGood();
            }

            if (PrintDebug)
                Debug.Log(carsOnTiles.Count);

            return carsOnTiles;
        }

        /// <summary>
        /// This method returns a list of cars which are on a tile contained by the list of tiles that a passed as parameter
        /// </summary>
        /// <param name="tiles">list of tiles to check if a car is currently on</param>
        /// <returns></returns>
        private List<CarBehaviour> getCarsOnTiles(List<TileBehaviour> tiles)
        {
            List<CarBehaviour> carsSpawned = new List<CarBehaviour>(GetComponentsInChildren<CarBehaviour>());
            List<CarBehaviour> carsOnTiles = new List<CarBehaviour>();

            foreach (CarBehaviour car in carsSpawned)
            {
                foreach (TileBehaviour tile in tiles)
                {
                    if (car.CurrentTile == tile)
                    {
                        carsOnTiles.Add(car);
                        break;
                    }  
                }
            }

            return carsOnTiles;
        }
    }
}
