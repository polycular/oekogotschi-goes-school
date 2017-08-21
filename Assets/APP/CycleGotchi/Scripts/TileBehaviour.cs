using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CycleGotchi
{
    public class TileBehaviour : MonoBehaviour
    {
        #region Public Fields

        public StreetGridBehaviour ParentGrid = null;
        [Tooltip("Link child GameObjects that should be enabled and scaled from 0 to 1 when the Tile turns good")]
        public List<GameObject> Growables = new List<GameObject>();
        [Tooltip("Link child Renderers that should beblended from 0 to 1 when the Tile turns good")]
        public List<Renderer> Blendables = new List<Renderer>();
        [Tooltip("If set, this transform will be rotated to a random multiple of 90° at start")]
        public Transform RandomizedRotationTarget = null;
        [Tooltip("Enables/Disables the rotation randomization")]
        public bool RandomizeRotation = false;

        #endregion

        #region Childtypes

        public enum Edge
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }

        public enum Corner
        {
            NORTH_EAST,
            SOUTH_EAST,
            SOUTH_WEST,
            NORTH_WEST
        }

        #endregion

        #region Static

        /// <summary>
        /// Wideness of the street on the edges of the tile
        /// Relative to the cale of the GameObject with the TileBehaviour
        /// </summary>
        public const float StreetPadding = 0.2f;

        /// <summary>
        /// Calculated with StreetPadding
        /// Returns the distance of the tile center to the middle of any street/lane on the tile.
        /// </summary>
        static public float CenterToLaneMiddle
        {
            get
            {
                return 0.5f - (StreetPadding * 0.5f);
            }
        }

        static public Corner getOppositeCorner(Corner corner)
        {
            switch (corner)
            {
                case Corner.NORTH_WEST:
                    return Corner.SOUTH_EAST;
                case Corner.NORTH_EAST:
                    return Corner.SOUTH_WEST;
                case Corner.SOUTH_WEST:
                    return Corner.NORTH_EAST;
                case Corner.SOUTH_EAST:
                    return Corner.NORTH_WEST;
                default:
                    return Corner.NORTH_EAST;
            }
        }

        static public Edge getOppositeEdge(Edge edge)
        {
            switch (edge)
            {
                case Edge.NORTH:
                    return Edge.SOUTH;
                case Edge.EAST:
                    return Edge.WEST;
                case Edge.SOUTH:
                    return Edge.NORTH;
                case Edge.WEST:
                    return Edge.EAST;
                default:
                    return Edge.NORTH;
            }
        }

        static public Edge getRightHandEdge(Corner corner)
        {
            switch (corner)
            {
                case Corner.NORTH_EAST:
                    return Edge.EAST;
                case Corner.SOUTH_EAST:
                    return Edge.SOUTH;
                case Corner.SOUTH_WEST:
                    return Edge.WEST;
                case Corner.NORTH_WEST:
                    return Edge.NORTH;
                default:
                    return Edge.NORTH;
            }
        }

        static public Edge getRightHandEdge(Edge edge)
        {
            switch (edge)
            {
                case Edge.NORTH:
                    return Edge.EAST;
                case Edge.EAST:
                    return Edge.SOUTH;
                case Edge.SOUTH:
                    return Edge.WEST;
                case Edge.WEST:
                    return Edge.NORTH;
                default:
                    return Edge.NORTH;
            }
        }

        /// <summary>
        /// Returns a position on the intersection of two streets (at the corner) of a tile
        /// Uses streetpaddings
        /// </summary>
        /// <param name="corner"></param>
        /// <returns></returns>
        static public Vector3 getRelativePositionOf(Corner corner)
        {
            float laneMiddle = CenterToLaneMiddle;
            switch (corner)
            {
                case Corner.NORTH_EAST:
                    return new Vector3(laneMiddle, 0, laneMiddle);
                case Corner.NORTH_WEST:
                    return new Vector3(-laneMiddle, 0, laneMiddle);
                case Corner.SOUTH_EAST:
                    return new Vector3(laneMiddle, 0, -laneMiddle);
                case Corner.SOUTH_WEST:
                    return new Vector3(-laneMiddle, 0, -laneMiddle);
                default:
                    return Vector3.zero;
            }
        }

        static public Vector3 getRelativePositionOfOuter(Corner corner)
        {
            switch (corner)
            {
                case Corner.NORTH_EAST:
                    return new Vector3(0.5f, 0, 0.5f);
                case Corner.NORTH_WEST:
                    return new Vector3(-0.5f, 0, 0.5f);
                case Corner.SOUTH_EAST:
                    return new Vector3(0.5f, 0, -0.5f);
                case Corner.SOUTH_WEST:
                    return new Vector3(-0.5f, 0, -0.5f);
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns a position on the street on the edge of a tile
        /// Uses streetpaddings
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        static public Vector3 getRelativePositionOf(Edge edge, float progress)
        {
            float laneMiddle = CenterToLaneMiddle;
            //Calculated from the lane progress, limited at start and end from the streets padding (never returns position on a crossing)
            float lanePosition = Mathf.Lerp(-0.5f + StreetPadding, 0.5f - StreetPadding, progress);

            switch (edge)
            {
                case Edge.NORTH:
                    return new Vector3(lanePosition, 0, laneMiddle);
                case Edge.EAST:
                    return new Vector3(laneMiddle, 0, -lanePosition);
                case Edge.SOUTH:
                    return new Vector3(-lanePosition, 0, -laneMiddle);
                case Edge.WEST:
                    return new Vector3(-laneMiddle, 0, lanePosition);
                default:
                    return Vector3.zero;
            }
        }

        static public Edge getEdgeCompassDirection(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            float angle = ToolbAR.Math.Vector3Extensions.getSignedAngleTo(new Vector3(1, 0, 1), direction, Vector3.up) + 180f;
            float quadrant = angle / 90f;
            if (quadrant > 3f)
            {
                return Edge.SOUTH;
            }
            else if (quadrant > 2f)
            {
                return Edge.EAST;
            }
            else if (quadrant > 1f)
            {
                return Edge.NORTH;
            }
            else
            {
                return Edge.WEST;
            }
        }

        static public Corner getCornerCompassDirection(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            float angle = ToolbAR.Math.Vector3Extensions.getSignedAngleTo(Vector3.forward, direction, Vector3.up) + 180f;
            float quadrant = angle / 90f;
            if (quadrant > 3f)
            {
                return Corner.SOUTH_EAST;
            }
            else if (quadrant > 2f)
            {
                return Corner.NORTH_EAST;
            }
            else if (quadrant > 1f)
            {
                return Corner.NORTH_WEST;
            }
            else
            {
                return Corner.SOUTH_WEST;
            }
        }

        #endregion

        #region Public Properties

        public float Goodness
        {
            get
            {
                return mGoodness;
            }
            set
            {
                mGoodness = Mathf.Clamp01(value);
            }
        }

        public bool IsGood
        {
            get
            {
                return mGoodness > 0f;
            }
        }
        /// <summary>
        /// Green is a synonym for "Good" on Tiles
        /// </summary>
        public bool IsGreen
        {
            get
            {
                return IsGood;
            }
        }
        public bool IsBad
        {
            get
            {
                return !IsGood;
            }
        }

        public bool IsTotallyGood
        {
            get
            {
                return mGoodness == 1f;
            }
        }
        public bool IsTotallyGreen
        {
            get
            {
                return IsTotallyGood;
            }
        }

        /// <summary>
        /// Returns true if the Tile is turning good, but is not totally good yet
        /// </summary>
        public bool IsTurningGood
        {
            get
            {
                return mIsTurningGood && mGoodness < 1f;
            }
        }
        public bool IsTurningGreen
        {
            get
            {
                return mIsTurningGood;
            }
        }

        #endregion

        #region Public Methods

        public int[] getIndex()
        {
            return StreetGridBehaviour.localPositionToIndex(this.transform.localPosition);
        }

        public Vector3 getPositionOfOuter(Corner corner)
        {
            if (transform.parent == null)
                return getLocalPositionOfOuter(corner);
            return transform.parent.TransformPoint(getLocalPositionOfOuter(corner));
        }
        public Vector3 getLocalPositionOfOuter(Corner corner)
        {
            return transform.localPosition + getRelativePositionOfOuter(corner);
        }

        public Vector3 getPositionOf(Corner corner)
        {
            if (transform.parent == null)
                return getLocalPositionOf(corner);
            return transform.parent.TransformPoint(getLocalPositionOf(corner));
        }
        public Vector3 getLocalPositionOf(Corner corner)
        {
            return transform.localPosition + getRelativePositionOf(corner);
        }
        public Vector3 getPositionOf(Edge edge, float progress)
        {
            if (transform.parent == null)
                return getLocalPositionOf(edge, progress);
            return transform.parent.TransformPoint(getLocalPositionOf(edge, progress));
        }
        public Vector3 getLocalPositionOf(Edge edge, float progress)
        {
            return transform.localPosition + getRelativePositionOf(edge, progress);
        }

        /// <summary>
        /// Returns the corner that faces the position most
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Corner getCornerFacingLocalPosition(Vector3 localPosition)
        {
            return getCornerCompassDirection(transform.localPosition, localPosition);
        }

        /// <summary>
        /// Returns the edge that faces the position most
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Edge getEdgeFacingLocalPosition(Vector3 localPosition)
        {
            return getEdgeCompassDirection(transform.localPosition, localPosition);
        }

        public void turnGood()
        {
            mIsTurningGood = true;
        }

        public TileBehaviour getNeighbour(TileBehaviour.Edge edgeToNeighbour)
        {
            return ParentGrid.getTileNeighbour(this, edgeToNeighbour);
        }
        public TileBehaviour getNeighbour(TileBehaviour.Corner cornerToNeighbour)
        {
            return ParentGrid.getTileNeighbour(this, cornerToNeighbour);
        }

        public TileBehaviour getNorthNeighbour()
        {
            return getNeighbour(Edge.NORTH);
        }
        public TileBehaviour getNorthEastNeighbour()
        {
            return getNeighbour(Corner.NORTH_EAST);
        }
        public TileBehaviour getEastNeighbour()
        {
            return getNeighbour(Edge.EAST);
        }
        public TileBehaviour getSouthEastNeighbour()
        {
            return getNeighbour(Corner.SOUTH_EAST);
        }
        public TileBehaviour getSouthNeighbour()
        {
            return getNeighbour(Edge.SOUTH);
        }
        public TileBehaviour getSouthWestNeighbour()
        {
            return getNeighbour(Corner.SOUTH_WEST);
        }
        public TileBehaviour getWestNeighbour()
        {
            return getNeighbour(Edge.WEST);
        }
        public TileBehaviour getNorthWestNeighbour()
        {
            return getNeighbour(Corner.NORTH_WEST);
        }

        #endregion

        #region Private Fields

        [Range(0f, 1f), SerializeField]
        float mGoodness = 0f;

        bool mIsTurningGood = false;

        #endregion

        #region Private Methods

        void resetGrowables()
        {
            growGrowables(0f);
        }

        void growGrowables(float progress)
        {
            progress = Mathf.Clamp01(progress);
            bool enable = (progress > 0f);
            foreach (GameObject growable in Growables)
            {
                growable.SetActive(enable);
                growable.transform.localScale = Vector3.one * progress;
            }
        }

        void randomizeTransformRotation()
        {
            if (RandomizeRotation && RandomizedRotationTarget != null)
            {
                float degree = 90f * (float)Random.Range(0, 4);
                RandomizedRotationTarget.Rotate(Vector3.up, degree);
            }
        }

        #endregion

        #region Unity Messages / Events

        void Start()
        {
            randomizeTransformRotation();
            resetGrowables();
        }

        void Update()
        {
            if (IsTurningGood)
            {
                Goodness += Time.deltaTime;
            }
            growGrowables(mGoodness);
        }

        void OnDrawGizmos()
        {
            //Debug Lane and Crossings
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(getPositionOf(Edge.NORTH, 0f), getPositionOf(Edge.NORTH, 1f));
            Gizmos.DrawWireCube(getPositionOf(Corner.NORTH_EAST), Vector3.one * 0.1f);
            Gizmos.DrawLine(getPositionOf(Edge.EAST, 0f), getPositionOf(Edge.EAST, 1f));
            Gizmos.DrawWireCube(getPositionOf(Corner.SOUTH_EAST), Vector3.one * 0.1f);
            Gizmos.DrawLine(getPositionOf(Edge.SOUTH, 0f), getPositionOf(Edge.SOUTH, 1f));
            Gizmos.DrawWireCube(getPositionOf(Corner.SOUTH_WEST), Vector3.one * 0.1f);
            Gizmos.DrawLine(getPositionOf(Edge.WEST, 0f), getPositionOf(Edge.WEST, 1f));
            Gizmos.DrawWireCube(getPositionOf(Corner.NORTH_WEST), Vector3.one * 0.1f);
        }

        #endregion
    }
}