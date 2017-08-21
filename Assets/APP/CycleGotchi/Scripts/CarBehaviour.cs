using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ToolbAR;

namespace CycleGotchi
{
    public struct TileWithEdge
    {
        public TileBehaviour Tile;
        public TileBehaviour.Edge Edge;
    }

    public struct Intersection
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Way;

        public float WayProgress;
    }

    public class CarBehaviour : MonoBehaviour
    {
        #region Public Fields

        /// <summary>
        /// The Tile the car is currently on. 
        /// </summary>
        public TileBehaviour CurrentTile;

        /// <summary>
        /// The lane on the CurrentTile the car is currently on.
        /// </summary>
        public TileBehaviour.Edge CurrentEdge;

        /// <summary>
        /// The progress on the current edge/lane. [0..1] 1 meaning the edge/lane was completed.
        /// </summary>
        public float EdgeProgress;

        public float TimePerTileInSeconds = 2.0f;
        public float TimerPerIntersectionInSeconds = 0.5f;

        [Tooltip("Use this to initialize the car object inside the editor instead of codewise. If it stays checked, it was initialized correctly.")]
        public bool EditorInitialize = false;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the StreetGridBehaviour the car is currently driving on. 
        /// </summary>
        public StreetGridBehaviour ParentGrid
        {
            get
            {
                return CurrentTile.ParentGrid;
            }
        }

        public bool IsCarOnIntersection
        {
            get
            {
                return mIsCarOnIntersection;
            }
        }

        #endregion

        /// <summary>
        /// Returns true if the car has been initialized and set up correctly.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return mIsInitialized;
            }
        }

        #region Public Methods

        /// <summary>
        /// Method to initialize a car behaviour. Can be seen as the constructor. 
        /// Since each driving lane is one-way only, no direction needs to be provided. 
        /// </summary>
        /// <param name="startTile">The Tile the car should start driving on.</param>
        /// <param name="startEdge">The edge on the tile the car should start driving on. </param>
        public void initialize(TileBehaviour startTile, TileBehaviour.Edge startEdge)
        {
            if (startTile != null)
            {
                CurrentTile = startTile;
                CurrentEdge = startEdge;
                mIsInitialized = true;
            }
            else
                ToolbAR.LogAR.logWarning("This car remains uninitialized. Please provide a valid current start tile and edge in the initialize method. ", this, this);

        }

        /// <summary>
        /// Sets the driving progression on the current tile to the given float. 
        /// </summary>
        /// <param name="progress">[0..1] with 1 meaning full progress. The value will be clamped to [0..1] if a higher or lower value was provided. </param>
        public void setProgressOnTile(float progress)
        {
            mCurrentProgressOnTile = Mathf.Clamp01(progress);
        }
        #endregion

        #region Private Fields

        bool mIsInitialized = false;
        float mCurrentProgressOnTile = 0.0f;

        private Intersection mCurrentIntersection;
        private bool mIsCarOnIntersection = false;
        float mCurrentTimeOnIntersection = 0.0f;

        #endregion

        #region Private Methods
        /// <summary>
        /// Randomly decides which way the car takes on a crossing, while respecting the ruleset. 
        /// The car never takes a way it is not allowed to drive on, e.g. green tiles, wrong lanes etc.
        /// </summary>
        void handleCrossing()
        {

            //add all possible ways from the current edge and tile to the possible tiles and according possible edges lists. 
            List<TileWithEdge> possibleWays = getPossibleWays();
            
            //remove any invalid ways
            for (int i = possibleWays.Count - 1; i >= 0; i--)
            {
                if (!isWayValid(possibleWays[i].Tile, possibleWays[i].Edge))
                {
                    possibleWays.RemoveAt(i);
                }
            }

            //choose a way
            TileWithEdge chosenWay = new TileWithEdge();
            if (possibleWays.Count > 1)
            {
                //choose one of the remaining ways by random
                int randomIndex = Random.Range(0, possibleWays.Count);
                chosenWay = possibleWays[randomIndex];
            }
            else if (possibleWays.Count == 1)
                chosenWay = possibleWays[0];
            else
                LogAR.logError("This car has no valid way to go from here", this, this);


            initIntersection(CurrentTile, CurrentEdge, chosenWay.Tile, chosenWay.Edge);

            CurrentTile = chosenWay.Tile;
            CurrentEdge = chosenWay.Edge;
        }

        /// <summary>
        /// Initializes the intersection struct which is used to move the car inbetween two tiles (after leaving the current one and entering a new one)
        /// </summary>
        /// <param name="leavingTile"></param>
        /// <param name="leavingTileEdge"></param>
        /// <param name="enteringTile"></param>
        /// <param name="enteringTileEdge"></param>
        private void initIntersection(TileBehaviour leavingTile, TileBehaviour.Edge leavingTileEdge, TileBehaviour enteringTile, TileBehaviour.Edge enteringTileEdge)
        {
            mCurrentIntersection.Start = leavingTile.getPositionOf(leavingTileEdge, 1);
            mCurrentIntersection.End = enteringTile.getPositionOf(enteringTileEdge, 0);
            mCurrentIntersection.Way = mCurrentIntersection.End - mCurrentIntersection.Start;
            mCurrentIntersection.WayProgress = 0.0f;

            mIsCarOnIntersection = true;
        }

        /// <summary>
        /// Gets the world position of the Intersection vector from the leaving tile's end to the entering tile's start position at position [0..1]
        /// </summary>
        /// <param name="progress">how much of the Intersection vector is already progressed [0..1]</param>
        /// <returns></returns>
        private Vector3 getIntersectionPositionOf(float progress)
        {
            //progress needs to be inbetween 0 and 1
            float prog = Mathf.Clamp01(progress);

            Vector3 newPos = mCurrentIntersection.Start + mCurrentIntersection.Way * prog;
            return newPos;
        }

        /// <summary>
        /// Returns the Tile-Edge combinations for left turn, right turn and driving straight ahead.
        /// </summary>
        /// <returns></returns>
        List<TileWithEdge> getPossibleWays()
        {
            List<TileWithEdge> possibleWays = new List<TileWithEdge>();

            possibleWays.Add(getWayForLeftTurn());
            possibleWays.Add(getWayForStraightAhead());
            possibleWays.Add(getWayForRightTurn());

            return possibleWays;
        }

        TileWithEdge getWayForLeftTurn()
        {
            TileWithEdge leftTurnWay = new TileWithEdge();

            switch (CurrentEdge)
            {
                case TileBehaviour.Edge.NORTH:
                    leftTurnWay.Tile = CurrentTile.getNorthEastNeighbour();
                    leftTurnWay.Edge = TileBehaviour.Edge.WEST;
                    break;

                case TileBehaviour.Edge.EAST:
                    leftTurnWay.Tile = CurrentTile.getSouthEastNeighbour();
                    leftTurnWay.Edge = TileBehaviour.Edge.NORTH;
                    break;

                case TileBehaviour.Edge.SOUTH:
                    leftTurnWay.Tile = CurrentTile.getSouthWestNeighbour();
                    leftTurnWay.Edge = TileBehaviour.Edge.EAST;
                    break;
                case TileBehaviour.Edge.WEST:
                    leftTurnWay.Tile = CurrentTile.getNorthWestNeighbour();
                    leftTurnWay.Edge = TileBehaviour.Edge.SOUTH;
                    break;
            }
            return leftTurnWay;
        }

        TileWithEdge getWayForStraightAhead()
        {
            //going straight means keeping the edge, just changing the tile
            TileWithEdge straightWay = new TileWithEdge();
            straightWay.Edge = CurrentEdge;

            switch (CurrentEdge)
            {
                case TileBehaviour.Edge.NORTH:
                    straightWay.Tile = CurrentTile.getEastNeighbour();
                    break;

                case TileBehaviour.Edge.EAST:
                    straightWay.Tile = CurrentTile.getSouthNeighbour();
                    break;

                case TileBehaviour.Edge.SOUTH:
                    straightWay.Tile = CurrentTile.getWestNeighbour();

                    break;
                case TileBehaviour.Edge.WEST:
                    straightWay.Tile = CurrentTile.getNorthNeighbour();
                    break;
            }
            return straightWay;
        }
        TileWithEdge getWayForRightTurn()
        {
            //a right turn always means to stay on the same tile
            TileWithEdge rightTurnWay = new TileWithEdge();
            rightTurnWay.Tile = CurrentTile;

            switch (CurrentEdge)
            {
                case TileBehaviour.Edge.NORTH:
                    rightTurnWay.Edge = TileBehaviour.Edge.EAST;
                    break;

                case TileBehaviour.Edge.EAST:
                    rightTurnWay.Edge = TileBehaviour.Edge.SOUTH;
                    break;

                case TileBehaviour.Edge.SOUTH:
                    rightTurnWay.Edge = TileBehaviour.Edge.WEST;
                    break;
                case TileBehaviour.Edge.WEST:
                    rightTurnWay.Edge = TileBehaviour.Edge.NORTH;
                    break;
            }

            return rightTurnWay;
        }

        bool isWayValid(TileBehaviour tile, TileBehaviour.Edge edge)
        {
            if (tile == null)
                return false;
            if (tile.IsGood)
                return false;
            return true;
        }
        #endregion

        #region Unity Messages / Events

        void Update()
        {
            if (!mIsInitialized)
            {
                if (EditorInitialize)
                {
                    initialize(CurrentTile, CurrentEdge);
                    if (!mIsInitialized)
                        EditorInitialize = false;
                }
                else
                    return;
            }

            if (!mIsCarOnIntersection)
            {
                mCurrentProgressOnTile += Time.deltaTime / TimePerTileInSeconds;

                if (mCurrentProgressOnTile > 1.0f)
                {
                    //progress to next tile
                    mCurrentProgressOnTile = 0.0f;
                    handleCrossing();
                    return;
                }

                EdgeProgress = Mathf.Clamp01(mCurrentProgressOnTile);

                transform.position = CurrentTile.getPositionOf(CurrentEdge, EdgeProgress);
                //rotate in driving direction
                transform.LookAt(CurrentTile.getPositionOf(CurrentEdge, 1.0f), transform.up);
            }
            else
            {
                //car is on a intersection between two tiles right now

                mCurrentTimeOnIntersection += Time.deltaTime;

                if (mCurrentTimeOnIntersection > TimerPerIntersectionInSeconds)
                {
                    mCurrentTimeOnIntersection = 0.0f;
                    mIsCarOnIntersection = false;
                    return;
                }

                EdgeProgress = Mathf.Clamp01(mCurrentTimeOnIntersection / TimerPerIntersectionInSeconds);

                transform.position = getIntersectionPositionOf(EdgeProgress);
                transform.LookAt(mCurrentIntersection.End, transform.up);
            }

        }

        #endregion
    }
}