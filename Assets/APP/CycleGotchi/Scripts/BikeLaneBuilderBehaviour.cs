using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharedAssets.GuiMain;

namespace CycleGotchi
{
    /// <summary>
    /// This class handles the building of Biking Lanes by the user.
    /// It itself uses Input to define to-be-built biking lanes,
    /// as well as building it (one at a time).
    /// </summary>
    public class BikeLaneBuilderBehaviour : MonoBehaviour
    {
        public StreetGridBehaviour StreetGrid = null;

        public float MaximumInputDistance = 0.5f;
        public float MaximumInputEndDistance = 0.5f;
        public float MinimumInputDragDistance = 0.25f;

        public bool InputIsEnabled = true;

        public bool InputIsInProgress
        {
            get
            {
                return mInputIsInProgress;
            }
        }
        public bool InputHasValidStart
        {
            get
            {
                return mInputHasValidStart;
            }
        }
        /// <summary>
        /// Dragging into the right direction
        /// </summary>
        public bool InputHasPossibleEnd
        {
            get
            {
                return mInputHasPossibleEnd;
            }
        }
        /// <summary>
        /// Has possible end and mouse position is near it
        /// </summary>
        public bool InputHasValidEnd
        {
            get
            {
                return mInputHasValidEnd;
            }
        }

        /// <summary>
        /// Checks if a path is currently building
        /// </summary>
        public bool IsBuilding
        {
            get
            {
                return mIsBuilding;
            }
        }

        /// <summary>
        /// Returns if the build was completed in the latest frame
        /// </summary>
        public bool HasBuilt
        {
            get
            {
                return mBuildingDoneLastFrame;
            }
        }

        public TileBehaviour[,] AffectedTiles
        {
            get
            {
                return mAffectedTiles;
            }
        }

        public float BuildProgress
        {
            get
            {
                return mBuildProgress;
            }
        }

        public Vector3 BuildStart
        {
            get
            {
                return mCurrentStart.getTile(mCurrentStartMajorTile).getPositionOf(TileBehaviour.getOppositeCorner(mCurrentStartMajorTile));
            }
        }
        public Vector3 BuildTarget
        {
            get
            {
                return mCurrentEnd.getTile(mCurrentEndMajorTile).getPositionOf(TileBehaviour.getOppositeCorner(mCurrentEndMajorTile));
            }
        }
        /// <summary>
        /// The current head/fornt of the building path (calculated with progress)
        /// </summary>
        public Vector3 BuildCurrentHead
        {
            get
            {
                return BuildStart + (BuildTarget - BuildStart) * mBuildProgress;
            }
        }

        public StreetGridBehaviour.Crossing CurrentStart
        {
            get
            {
                return mCurrentStart;
            }
        }

        public StreetGridBehaviour.Crossing CurrentEnd
        {
            get
            {
                return mCurrentEnd;
            }
        }

        public void resetBuild()
        {
            mIsBuilding = false;
            mBuildProgress = 0f;
        }


        public static bool checkBorderTile(TileBehaviour tile)
        {
            return (tile == null || tile.IsGreen);
        }
        /// <summary>
        /// Returns true if the crossing contains a bordertile, but still has directions open
        /// </summary>
        /// <param name="crossing"></param>
        /// <returns></returns>
        public static bool checkValidStartCrossing(StreetGridBehaviour.Crossing crossing)
        {
            Dictionary<TileBehaviour.Edge, bool> possibleDirections = getPossibleBuildingDirections(crossing);
            return ((
                possibleDirections[TileBehaviour.Edge.NORTH] ||
                possibleDirections[TileBehaviour.Edge.EAST] ||
                possibleDirections[TileBehaviour.Edge.SOUTH] ||
                possibleDirections[TileBehaviour.Edge.WEST]
                ) && (
                    checkBorderTile(crossing.NorthEast) ||
                    checkBorderTile(crossing.NorthWest) ||
                    checkBorderTile(crossing.SouthEast) ||
                    checkBorderTile(crossing.SouthWest)
                ));

        }
        public static Dictionary<TileBehaviour.Edge, bool> getPossibleBuildingDirections(StreetGridBehaviour.Crossing startCrossing)
        {
            bool ne = checkBorderTile(startCrossing.NorthEast);
            bool nw = checkBorderTile(startCrossing.NorthWest);
            bool se = checkBorderTile(startCrossing.SouthEast);
            bool sw = checkBorderTile(startCrossing.SouthWest);

            Dictionary<TileBehaviour.Edge, bool> possible = new Dictionary<TileBehaviour.Edge, bool>();
            //Possible Directons
            possible[TileBehaviour.Edge.NORTH] = !(ne || nw);
            possible[TileBehaviour.Edge.EAST] = !(ne || se);
            possible[TileBehaviour.Edge.SOUTH] = !(se || sw);
            possible[TileBehaviour.Edge.WEST] = !(nw || sw);

            return possible;
        }

        /// <summary>
        /// Retruns true if it was possible to detemrine a starting position.
        /// </summary>
        /// <param name="currentStartPosition"></param>
        /// <returns></returns>
        public bool getCurrentStartPosition(out Vector3 currentStartPosition)
        {
            currentStartPosition = Vector3.one;

            if (!InputHasPossibleEnd)
            {
                return false;
            }

            TileBehaviour majorStartTile = mCurrentStart.getTile(mCurrentStartMajorTile);

            if (majorStartTile == null)
            {
                return false;
            }

            currentStartPosition = majorStartTile.getPositionOf(TileBehaviour.getOppositeCorner(mCurrentStartMajorTile));
            return true;
        }
        public bool getCurrentEndPosition(out Vector3 currentEndPosition)
        {
            currentEndPosition = Vector3.one;

            if (!InputHasPossibleEnd)
            {
                return false;
            }

            TileBehaviour majorEndTile = mCurrentEnd.getTile(mCurrentEndMajorTile);

            if (majorEndTile == null)
            {
                return false;
            }

            currentEndPosition = majorEndTile.getPositionOf(TileBehaviour.getOppositeCorner(mCurrentEndMajorTile));
            return true;

        }

        #region Private Methods

        Vector3 convertTouchToPosition(out bool hit)
        {
            Ray ray = GUIManagerBehaviour.InputManagerInstance.getLastPassedFingerRay(Camera.main);
            // create a plane at 0,0,0 whose normal points to +Y from the grid:
            Plane hPlane = new Plane(StreetGrid.transform.up, StreetGrid.transform.position);
            // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            float distance = 0;
            // if the ray hits the plane...
            if (hPlane.Raycast(ray, out distance))
            {
                // get the hit point:
                hit = true;
                return ray.GetPoint(distance);
            }

            hit = false;
            return ray.origin;
        }

        /// <summary>
        /// Handles all input interactions
        /// </summary>
        void handleInput()
        {
            if (!InputIsEnabled)
            {
                resetInput();
                return;
            }

            if (GUIManagerBehaviour.InputManagerInstance.passedFinger())
            {
                mInputIsInProgress = true;

                //Get the touch on the grid
                bool hasHit;
                Vector3 touchPoint = convertTouchToPosition(out hasHit);
#if UNITY_EDITOR
                mLatestTouch = touchPoint;
#endif
                if (!hasHit)
                {
                    //only continue if the touchPoint is really on the StreetGrid's Plane
                    return;
                }
                Vector3 localizedTouchPoint = StreetGrid.transform.InverseTransformPoint(touchPoint);

                if (!mInputHasValidStart)
                {
                    //Find a valid start first
                    mCurrentStart = StreetGrid.getClosestCrossingAtLocalPosition(localizedTouchPoint);

                    if (!checkValidStartCrossing(mCurrentStart))
                    {
                        return;
                    }

                    float localDistanceToCrossing = Vector3.Distance(StreetGrid.transform.InverseTransformPoint(mCurrentStart.Position), localizedTouchPoint);
                    if(localDistanceToCrossing > MaximumInputDistance)
                    {
                        return;
                    }

                    mInputHasValidStart = true;
                }
                else
                {
                    mInputHasPossibleEnd = false;
                    mInputHasValidEnd = false;
                    //See if we can take out a Direction where it should go
                    Vector3 dir = localizedTouchPoint - StreetGrid.transform.InverseTransformPoint(mCurrentStart.Position);
                    if (dir.magnitude >= MinimumInputDragDistance)
                    {
                        mInputCurrentDirection = TileBehaviour.getEdgeCompassDirection(StreetGrid.transform.InverseTransformPoint(mCurrentStart.Position), localizedTouchPoint);

                        Dictionary<TileBehaviour.Edge, bool> possibleDirs = getPossibleBuildingDirections(mCurrentStart);

                        if (possibleDirs[mInputCurrentDirection])
                        {
                            mCurrentEnd = getNewBikePath(out mInputHasPossibleEnd);
                        }

                        if (mInputHasPossibleEnd)
                        {
                            float localDistanceToCrossing = Vector3.Distance(StreetGrid.transform.InverseTransformPoint(mCurrentEnd.Position), localizedTouchPoint);
                            mInputHasValidEnd = (localDistanceToCrossing <= MaximumInputEndDistance);
                        }

                    }
                }
            }
            else if (GUIManagerBehaviour.InputManagerInstance.passedFingerUp())
            {
                //TODO: see if it was a valid finalization of the input, and start the build
                if (InputHasValidEnd)
                {
                    mIsBuilding = true;
                }
                resetInput();
            }
            else
            {
                resetInput();
            }
        }

        /// <summary>
        /// Returns a crossing that can edfine the end point to the given startPoint for a bikingPath
        /// </summary>
        /// <param name="validEndFound"></param>
        /// <returns></returns>
        StreetGridBehaviour.Crossing findNewBikePath(out bool validEndFound)
        {
            StreetGridBehaviour.Crossing head = mCurrentStart;
            validEndFound = false;
            while (!validEndFound && StreetGrid.getNextCrossing(head, mInputCurrentDirection, out head))
            {
                switch (mInputCurrentDirection)
                {
                    case TileBehaviour.Edge.NORTH:
                        if (checkBorderTile(head.NorthEast) || checkBorderTile(head.NorthWest))
                        {
                            validEndFound = true;
                        }
                        break;
                    case TileBehaviour.Edge.EAST:
                        if (checkBorderTile(head.NorthEast) || checkBorderTile(head.SouthEast))
                        {
                            validEndFound = true;
                        }
                        break;
                    case TileBehaviour.Edge.SOUTH:
                        if (checkBorderTile(head.SouthEast) || checkBorderTile(head.SouthWest))
                        {
                            validEndFound = true;
                        }
                        break;
                    case TileBehaviour.Edge.WEST:
                        if (checkBorderTile(head.NorthEast) || checkBorderTile(head.NorthWest))
                        {
                            validEndFound = true;
                        }
                        break;
                }
            }
            return head;
        }

        /// <summary>
        /// returns tiles and information that is used to flip (and preview) them, once a user create a new bikelane
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        TileBehaviour[,] decideOnTargetTiles(StreetGridBehaviour.Crossing from, StreetGridBehaviour.Crossing to, out TileBehaviour corner1, out TileBehaviour corner2)
        {
            //STUB

            //First, get the direction of the border
            TileBehaviour.Edge direction = TileBehaviour.getEdgeCompassDirection(StreetGrid.transform.InverseTransformPoint(from.Position), StreetGrid.transform.InverseTransformPoint(to.Position));

            //CornerTiles for first side
            TileBehaviour cornerA1;
            TileBehaviour cornerA2;
            //CornerTiles for second side
            TileBehaviour cornerB1;
            TileBehaviour cornerB2;

            //Get fitting cornerstones for the tow posible layouts of the path
            switch (direction)
            {
                case TileBehaviour.Edge.NORTH:
                    cornerA1 = from.NorthEast;
                    cornerA2 = to.SouthEast;
                    cornerB1 = from.NorthWest;
                    cornerB2 = to.SouthWest;
                    break;
                case TileBehaviour.Edge.SOUTH:
                    cornerA1 = from.SouthWest;
                    cornerA2 = to.NorthWest;
                    cornerB1 = from.SouthEast;
                    cornerB2 = to.NorthEast;
                    break;
                case TileBehaviour.Edge.EAST:
                    cornerA1 = from.SouthEast;
                    cornerA2 = to.SouthWest;
                    cornerB1 = from.NorthEast;
                    cornerB2 = to.NorthWest;
                    break;
                case TileBehaviour.Edge.WEST:
                    cornerA1 = from.NorthWest;
                    cornerA2 = to.NorthEast;
                    cornerB1 = from.SouthWest;
                    cornerB2 = to.SouthEast;
                    break;
                default:
                    throw new System.Exception("Missing Configuration for Edge value in BikeLaneBuilderBehaviour::decideOnTargetTiles");
            }

            TileBehaviour[,] tilesA, tilesB;
            if (cornerA1 == cornerA2)
            {
                //Bugix: 1 tile length border have nothing to calculate the right handed direction with
                tilesA = StreetGrid.selectTileArea(cornerA1, TileBehaviour.getRightHandEdge(direction));
            }
            else
            {
                tilesA = StreetGrid.selectTileArea(cornerA1, cornerA2);
            }
            if (cornerA1 == cornerA2)
            {
                //Bugix: 1 tile length border have nothing to calculate the right handed direction with
                tilesB = StreetGrid.selectTileArea(cornerB1, TileBehaviour.getRightHandEdge(TileBehaviour.getOppositeEdge(direction)));
            }
            else
            {
                tilesB = StreetGrid.selectTileArea(cornerB2, cornerB1);
            }

            int badInA = 0;
            int badInB = 0;

            foreach (TileBehaviour tile in tilesA)
            {
                if (tile.IsBad)
                    badInA++;
            }
            foreach (TileBehaviour tile in tilesB)
            {
                if (tile.IsBad)
                    badInB++;
            }

            if (badInA < badInB)
            {
                corner1 = cornerA1;
                corner2 = cornerA2;
                return tilesA;
            }
            else if (badInB < badInA)
            {

                corner1 = cornerB1;
                corner2 = cornerB2;
                return tilesB;
            }
            else if (tilesA.Length > tilesB.Length)
            {
                corner1 = cornerB1;
                corner2 = cornerB2;
                return tilesB;
            }
            else
            {
                corner1 = cornerA1;
                corner2 = cornerA2;
                return tilesA;
            }
        }

        /// <summary>
        /// Checks and sets if there is a possible new path to be built
        /// </summary>
        /// <param name="validEndFound"></param>
        /// <returns></returns>
        StreetGridBehaviour.Crossing getNewBikePath(out bool validEndFound)
        {
            StreetGridBehaviour.Crossing end = findNewBikePath(out validEndFound);
            if (validEndFound)
            {
                TileBehaviour tile1;
                TileBehaviour tile2;
                mAffectedTiles = decideOnTargetTiles(mCurrentStart, end, out tile1, out tile2);

                if (mCurrentStart.NorthEast == tile1)
                    mCurrentStartMajorTile = TileBehaviour.Corner.NORTH_EAST;
                else if (mCurrentStart.NorthWest == tile1)
                    mCurrentStartMajorTile = TileBehaviour.Corner.NORTH_WEST;
                else if (mCurrentStart.SouthEast == tile1)
                    mCurrentStartMajorTile = TileBehaviour.Corner.SOUTH_EAST;
                else if (mCurrentStart.SouthWest == tile1)
                    mCurrentStartMajorTile = TileBehaviour.Corner.SOUTH_WEST;

                if (mCurrentEnd.NorthEast == tile2)
                    mCurrentEndMajorTile = TileBehaviour.Corner.NORTH_EAST;
                else if (mCurrentEnd.NorthWest == tile2)
                    mCurrentEndMajorTile = TileBehaviour.Corner.NORTH_WEST;
                else if (mCurrentEnd.SouthEast == tile2)
                    mCurrentEndMajorTile = TileBehaviour.Corner.SOUTH_EAST;
                else if (mCurrentEnd.SouthWest == tile2)
                    mCurrentEndMajorTile = TileBehaviour.Corner.SOUTH_WEST;
            }


            return end;
        }

        void resetInput()
        {
            mInputIsInProgress = false;
            mInputHasValidStart = false;
            mInputHasValidEnd = false;
            mInputHasPossibleEnd = false;
        }
        #endregion

        #region Private Fields

        bool mInputIsInProgress = false;
        bool mInputHasValidStart = false;
        bool mInputHasValidEnd = false;
        bool mInputHasPossibleEnd = false;
        TileBehaviour.Edge mInputCurrentDirection;
        StreetGridBehaviour.Crossing mCurrentStart;
        StreetGridBehaviour.Crossing mCurrentEnd;
        TileBehaviour.Corner mCurrentStartMajorTile;
        TileBehaviour.Corner mCurrentEndMajorTile;

        TileBehaviour[,] mAffectedTiles;


        bool mIsBuilding = false;
        bool mBuildingDoneLastFrame = false;
        float mBuildProgress = 0f;

        #endregion

        #region Unity Messages / Events

        void OnEnable()
        {
        }

        void Start()
        {
        }

        void Update()
        {
            mBuildingDoneLastFrame = false;
            if (mIsBuilding)
            {
                mBuildProgress = Mathf.Clamp01(mBuildProgress + Time.deltaTime * 0.5f);
                if (mBuildProgress == 1f)
                {
                    mBuildingDoneLastFrame = true;
                    resetBuild();
                }
            }
            else
            {
                handleInput();
            }
        }

        void OnDisable()
        {
            resetInput();
        }

#if UNITY_EDITOR
        Vector3 mLatestTouch;
        void OnDrawGizmos()
        {

            if(IsBuilding)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(BuildStart, Vector3.one * TileBehaviour.StreetPadding * 2f);
                Gizmos.DrawLine(BuildStart, BuildCurrentHead);
                Gizmos.DrawCube(BuildCurrentHead, Vector3.one * TileBehaviour.StreetPadding * 2f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(BuildCurrentHead, BuildTarget);
                Gizmos.DrawCube(BuildTarget, Vector3.one * TileBehaviour.StreetPadding * 2f);
            }
            else if (InputIsInProgress)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(mLatestTouch, 0.1f);
                Color good = Color.cyan;
                good.a = 0.8f;
                Color bad = new Color(1f, 0.63f, 0f, 0.8f);


                Gizmos.color = (InputHasValidStart) ? good : bad;
                Gizmos.DrawCube(mCurrentStart.Position, Vector3.one * TileBehaviour.StreetPadding * 2f);
                if (!InputHasValidStart)
                {
                    Gizmos.DrawWireCube(mCurrentStart.Position, StreetGrid.transform.lossyScale * MaximumInputDistance);
                }
                if (InputHasPossibleEnd)
                {

                    Gizmos.color = (InputHasValidEnd) ? good : bad;
                    Gizmos.DrawLine(mCurrentStart.Position, mCurrentEnd.Position);
                    Gizmos.DrawCube(mCurrentEnd.Position, Vector3.one * TileBehaviour.StreetPadding * 2f);

                    if (!InputHasValidEnd)
                    {
                        Gizmos.DrawWireCube(mCurrentEnd.Position, StreetGrid.transform.lossyScale * MaximumInputEndDistance);
                    }

                    TileBehaviour majorStartTile = mCurrentStart.getTile(mCurrentStartMajorTile);

                    if (majorStartTile != null)
                    {
                        Vector3 targetCornerPosition = majorStartTile.getPositionOf(TileBehaviour.getOppositeCorner(mCurrentStartMajorTile));
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(targetCornerPosition, Vector3.one * TileBehaviour.StreetPadding);

                        TileBehaviour majorEndTile = mCurrentEnd.getTile(mCurrentEndMajorTile);
                        if (majorEndTile != null)
                        {
                            Vector3 endTargetCornerPosition = majorEndTile.getPositionOf(TileBehaviour.getOppositeCorner(mCurrentEndMajorTile));
                            Gizmos.color = Color.black;
                            Gizmos.DrawCube(endTargetCornerPosition, Vector3.one * TileBehaviour.StreetPadding);
                            
                            Gizmos.DrawLine(targetCornerPosition, endTargetCornerPosition);
                        }
                    }

                }

            }

        }
#endif

        #endregion
    }
}