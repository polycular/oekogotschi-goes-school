using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace CycleGotchi
{
    public class StreetGridBehaviour : MonoBehaviour
    {
        #region Public Fields
        
        public int GridColumns = 5;
        public int GridRows = 5;

        public List<TileBehaviour> TilePrefabs = new List<TileBehaviour>();

        #endregion

        #region Childtypes

        //Defines the crossing of streets
        public struct Crossing
        {
            public Crossing(Vector3 pos, TileBehaviour NE, TileBehaviour SE, TileBehaviour SW, TileBehaviour NW)
            {
                Position = pos;
                NorthEast = NE;
                SouthEast = SE;
                SouthWest = SW;
                NorthWest = NW;
            }

            public Vector3 Position;

            public TileBehaviour NorthEast;
            public TileBehaviour SouthEast;
            public TileBehaviour SouthWest;
            public TileBehaviour NorthWest;

            public TileBehaviour getTile(TileBehaviour.Corner corner)
            {
                switch (corner)
                {
                    case TileBehaviour.Corner.NORTH_EAST:
                        return NorthEast;
                    case TileBehaviour.Corner.SOUTH_EAST:
                        return SouthEast;
                    case TileBehaviour.Corner.NORTH_WEST:
                        return NorthWest;
                    case TileBehaviour.Corner.SOUTH_WEST:
                        return SouthWest;
                    default:
                        return null;
                }
            }

            public override string ToString()
            {
                return "CycleGotchi.StreetGridBehaviour.Crossing{NE:" + NorthEast + ", SE:" + SouthEast + ", SW:" + SouthWest + ", NW:" + NorthWest + "}@" + Position + "";
            }
        }

        #endregion

        #region Public Properties

        public TileBehaviour[,] Tiles
        {
            get
            {
                return mTiles;
            }
        }

        #endregion

        #region Public Methods

        public bool recreateIfNecessary()
        {
            recollectGrid();
            if (mTiles != null && mTiles.Length > 0)
            {
                return false;
            }
            else
            {
                recreateGrid();
                return true;
            }
        }

        static public int[] localPositionToIndex(Vector3 localPosition)
        {
            int[] idx = new int[2];
            //x
            idx[0] = Mathf.FloorToInt(localPosition.x + 0.5f);
            //y
            idx[1] = Mathf.FloorToInt(localPosition.z + 0.5f);

            return idx;
        }

        public Crossing getCrossing(TileBehaviour tile, TileBehaviour.Corner corner)
        {
            switch(corner)
            {
                case TileBehaviour.Corner.NORTH_EAST:
                    return new Crossing(
                        tile.getPositionOfOuter(corner),
                        tile.getNorthEastNeighbour(),
                        tile.getEastNeighbour(),
                        tile,
                        tile.getNorthNeighbour()
                        );
                case TileBehaviour.Corner.NORTH_WEST:
                    return new Crossing(
                        tile.getPositionOfOuter(corner),
                        tile.getNorthNeighbour(),
                        tile,
                        tile.getWestNeighbour(),
                        tile.getNorthWestNeighbour()
                        );
                case TileBehaviour.Corner.SOUTH_EAST:
                    return new Crossing(
                        tile.getPositionOfOuter(corner),
                        tile.getEastNeighbour(),
                        tile.getSouthEastNeighbour(),
                        tile.getSouthNeighbour(),
                        tile
                        );
                case TileBehaviour.Corner.SOUTH_WEST:
                    return new Crossing(
                        tile.getPositionOfOuter(corner),
                        tile,
                        tile.getSouthNeighbour(),
                        tile.getSouthWestNeighbour(),
                        tile.getWestNeighbour()
                        );
                default:
                    return new Crossing(Vector3.zero, null, null, null, null);

            }
        }

        public bool getNextCrossing(Crossing pivot, TileBehaviour.Edge direction, out Crossing next)
        {
            switch(direction)
            {
                case TileBehaviour.Edge.NORTH:
                    if (pivot.NorthEast != null)
                    {
                        next = getCrossing(pivot.NorthEast, TileBehaviour.Corner.NORTH_WEST);
                        return true;
                    }
                    if (pivot.NorthWest != null)
                    {
                        next = getCrossing(pivot.NorthWest, TileBehaviour.Corner.NORTH_EAST);
                        return true;
                    }
                    break;
                case TileBehaviour.Edge.EAST:
                    if (pivot.NorthEast != null)
                    {
                        next = getCrossing(pivot.NorthEast, TileBehaviour.Corner.SOUTH_EAST);
                        return true;
                    }
                    if (pivot.SouthEast != null)
                    {
                        next = getCrossing(pivot.SouthEast, TileBehaviour.Corner.NORTH_EAST);
                        return true;
                    }
                    break;
                case TileBehaviour.Edge.SOUTH:
                    if (pivot.SouthEast != null)
                    {
                        next = getCrossing(pivot.SouthEast, TileBehaviour.Corner.SOUTH_WEST);
                        return true;
                    }
                    if (pivot.SouthWest != null)
                    {
                        next = getCrossing(pivot.SouthWest, TileBehaviour.Corner.SOUTH_EAST);
                        return true;
                    }
                    break;
                case TileBehaviour.Edge.WEST:
                    if (pivot.NorthWest != null)
                    {
                        next = getCrossing(pivot.NorthWest, TileBehaviour.Corner.SOUTH_WEST);
                        return true;
                    }
                    if (pivot.SouthWest != null)
                    {
                        next = getCrossing(pivot.SouthWest, TileBehaviour.Corner.NORTH_WEST);
                        return true;
                    }
                    break;
            }
            next = new Crossing(Vector3.zero, null, null, null, null);
            return false;

        }

        public TileBehaviour getTile(int column, int row)
        {
            if (mTiles != null && mTiles.GetLength(0) > column && column >= 0 && mTiles.GetLength(1) > row && row >= 0)
            {
                return mTiles[column, row];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// If the indices are out of bounds, restrain them to bounds
        /// Can still return null if there are no tiles
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public TileBehaviour getClosestTile(int column, int row)
        {
            if (mTiles == null)
                return null;

            if (column >= mTiles.GetLength(0))
                column = mTiles.GetLength(0) - 1;
            else if (column < 0)
                column = 0;
            if (row >= mTiles.GetLength(1))
                row = mTiles.GetLength(1) - 1;
            else if (row < 0)
                row = 0;
            return mTiles[column, row];
        }

        public Crossing getClosestCrossingAtLocalPosition(Vector3 localPosition)
        {
            TileBehaviour closestTile = getClosestTileAtLocalPosition(localPosition);
            TileBehaviour.Corner closestCorner = closestTile.getCornerFacingLocalPosition(localPosition);

            return getCrossing(closestTile, closestCorner);
        }

        public TileBehaviour getTileAtLocalPosition(Vector3 localPosition)
        {
            int[] idx = localPositionToIndex(localPosition);
            return getTile(idx[0], idx[1]);
        }
        public TileBehaviour getClosestTileAtLocalPosition(Vector3 localPosition)
        {
            int[] idx = localPositionToIndex(localPosition);
            return getClosestTile(idx[0], idx[1]);
        }

        public Crossing getClosestCrossingAtPosition(Vector3 position)
        {
            return getClosestCrossingAtLocalPosition(transform.InverseTransformPoint(position));
        }

        public TileBehaviour getTileAtPosition(Vector3 position)
        {
            return getTileAtLocalPosition(transform.InverseTransformPoint(position));
        }

        public TileBehaviour getClosestTileAtPosition(Vector3 position)
        {
            return getClosestTileAtLocalPosition(transform.InverseTransformPoint(position));
        }

        public TileBehaviour getTileNeighbour(TileBehaviour pivot, TileBehaviour.Edge edgeToNeighbour)
        {
            int[] idx = localPositionToIndex(pivot.transform.localPosition);

            switch (edgeToNeighbour)
            {
                case TileBehaviour.Edge.NORTH:
                    return getTile(idx[0], idx[1] + 1);
                case TileBehaviour.Edge.EAST:
                    return getTile(idx[0] + 1, idx[1]);
                case TileBehaviour.Edge.SOUTH:
                    return getTile(idx[0], idx[1] - 1);
                case TileBehaviour.Edge.WEST:
                    return getTile(idx[0] - 1, idx[1]);
                default:
                    return null;
            }
        }

        public TileBehaviour getTileNeighbour(TileBehaviour pivot, TileBehaviour.Corner cornerToNeighbour)
        {
            int[] idx = localPositionToIndex(pivot.transform.localPosition);

            switch (cornerToNeighbour)
            {
                case TileBehaviour.Corner.NORTH_EAST:
                    return getTile(idx[0] + 1, idx[1] + 1);
                case TileBehaviour.Corner.SOUTH_EAST:
                    return getTile(idx[0] + 1, idx[1] - 1);
                case TileBehaviour.Corner.SOUTH_WEST:
                    return getTile(idx[0] - 1, idx[1] - 1);
                case TileBehaviour.Corner.NORTH_WEST:
                    return getTile(idx[0] - 1, idx[1] + 1);
                default:
                    return null;
            }
        }

        public TileBehaviour getTileNorthFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Edge.NORTH);
        }

        public TileBehaviour getTileNorthEastFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Corner.NORTH_EAST);
        }

        public TileBehaviour getTileEastFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Edge.EAST);
        }

        public TileBehaviour getTileSouthEastFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Corner.SOUTH_EAST);
        }

        public TileBehaviour getTileSouthFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Edge.SOUTH);
        }

        public TileBehaviour getTileSouthWestFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Corner.SOUTH_WEST);
        }

        public TileBehaviour getTileWestFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Edge.WEST);
        }

        public TileBehaviour getTileNorthWestFrom(TileBehaviour pivot)
        {
            return getTileNeighbour(pivot, TileBehaviour.Corner.NORTH_WEST);
        }

        public TileBehaviour[,] getTileRange(int fromColumn, int fromRow, int toColumn, int toRow)
        {
            if (fromColumn > toColumn)
            {
                int temp = fromColumn;
                fromColumn = toColumn;
                toColumn = temp;
            }
            if (fromRow > toRow)
            {
                int temp = fromRow;
                fromRow = toRow;
                toRow = temp;
            }

            TileBehaviour[,] result = new TileBehaviour[1 + (toColumn - fromColumn), 1 + (toRow - fromRow)];
            for (int x = fromColumn; x <= toColumn; x++)
            {
                for (int y = fromRow; y <= toRow; y++)
                {
                    result[x - fromColumn, y - fromRow] = getTile(x, y);
                }
            }
            return result;
        }

        /// <summary>
        /// Selects all tiles from (including) the pivot, and into the direction thats given
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public TileBehaviour[,] selectTileArea(TileBehaviour pivot, TileBehaviour.Edge direction)
        {
            int[] startIdx = pivot.getIndex();
            int[] targetIdx = pivot.getIndex();

            switch (direction)
            {
                case TileBehaviour.Edge.NORTH:
                    targetIdx[1] = mTiles.GetLength(0) - 1;
                    break;
                case TileBehaviour.Edge.EAST:
                    targetIdx[0] = mTiles.GetLength(1) - 1;
                    break;
                case TileBehaviour.Edge.SOUTH:
                    targetIdx[1] = 0;
                    break;
                case TileBehaviour.Edge.WEST:
                    targetIdx[0] = 0;
                    break;

            }
            return getTileRange(startIdx[0], startIdx[1], targetIdx[0], targetIdx[1]);
        }
        /// <summary>
        /// Selects all tiles from (including'from to 'to' (also included) to the end of the map
        /// The direction is the right-hand direction of the directon from=>to
        /// This menas that its calculation is influenced by input order
        /// 
        /// Please be sure tht the tiles are parallel
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public TileBehaviour[,] selectTileArea(TileBehaviour from, TileBehaviour to)
        {
            int[] startIdx = from.getIndex();
            int[] targetIdx = to.getIndex();

            TileBehaviour.Edge direction = TileBehaviour.getRightHandEdge(from.getEdgeFacingLocalPosition(to.transform.localPosition));

            switch (direction)
            {
                case TileBehaviour.Edge.NORTH:
                    targetIdx[1] = mTiles.GetLength(0) - 1;
                    break;
                case TileBehaviour.Edge.EAST:
                    targetIdx[0] = mTiles.GetLength(1) - 1;
                    break;
                case TileBehaviour.Edge.SOUTH:
                    targetIdx[1] = 0;
                    break;
                case TileBehaviour.Edge.WEST:
                    targetIdx[0] = 0;
                    break;

            }
            return getTileRange(startIdx[0], startIdx[1], targetIdx[0], targetIdx[1]);
        }

        public void recreateGrid()
        {
            generateNewTiles();
        }

        public void recollectGrid()
        {
            TileBehaviour[] childTiles = GetComponentsInChildren<TileBehaviour>(true);

            if (childTiles.Length > 0)
            {
                mTiles = new TileBehaviour[GridColumns, GridRows];

                foreach (TileBehaviour tile in childTiles)
                {
                    int[] idx = localPositionToIndex(tile.transform.localPosition);
                    mTiles[idx[0], idx[1]] = tile;
                }
            }
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Column-major 2D Array
        /// </summary>
        TileBehaviour[,] mTiles = null;

        #endregion

        #region Private Methods

        TileBehaviour selectRandomTilePrefab()
        {
            if (TilePrefabs.Count == 0)
                return null;

            int randIdx = Random.Range(0, TilePrefabs.Count);
            return TilePrefabs[randIdx];
        }

        void generateNewTiles()
        {
            destructAllTiles();

            mTiles = new TileBehaviour[GridColumns, GridRows];
            for (int x = 0; x < GridColumns; x++)
            {
                for (int y = 0; y < GridRows; y++)
                {
                    GameObject prefab = selectRandomTilePrefab().gameObject;
                    GameObject go = Instantiate(prefab) as GameObject;
                    TileBehaviour tile = go.GetComponent<TileBehaviour>();
                    go.transform.parent = this.transform;
                    go.transform.localPosition = new Vector3(x, 0, y);
                    go.transform.localScale = Vector3.one; // to compensate for the 0.5 on the game director
                    go.transform.localRotation = Quaternion.identity;
                    tile.ParentGrid = this;
                    go.name = prefab.name + "[" + x + "," + y + "]";
                    mTiles[x, y] = tile;
                }
            }
        }

        void destructAllTiles()
        {
            if (mTiles == null)
                return;

            foreach (TileBehaviour tile in mTiles)
            {
                if (tile != null && tile.gameObject != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }

        void collectGrid()
        {
            destructAllTiles();

            recollectGrid();
        }

        #endregion

        #region Unity Messages / Events

        void Awake()
        {
            recollectGrid();
        }


        #endregion
    }
}