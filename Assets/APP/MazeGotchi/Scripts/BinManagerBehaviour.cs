using UnityEngine;
using System.Collections;

namespace MazeGotchi
{
    public class BinManagerBehaviour : MonoBehaviour
    {
        public enum BinType { REST, BIO, PAPIER, METALL, PLASTIK };
        public GameObject BinPrefab1, BinPrefab2, BinPrefab3, BinPrefab4, BinPrefab5;
        public GameObject BinParent;
        public Transform[] BinPositions, BinPositionExits;
        public float MoveTimeInSeconds = 0.2f;

        private bool mHasSpawned = false, mIsCurrentlyMoving = false;
        private GameObject[] mBins;
        private GameObject mHiddenBin = null;
        private Vector3[] mOriginPositions;
        private Vector3[] mTargetPositions;
        private float mTimeSinceMovementStarted;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!mHasSpawned)
                return;

            if (mIsCurrentlyMoving)
            {
                mTimeSinceMovementStarted += Time.deltaTime;
                float positionlerp_Tvalue = mTimeSinceMovementStarted / MoveTimeInSeconds;
                if (positionlerp_Tvalue > 1)
                    positionlerp_Tvalue = 1;

                //move the bins
                for (int i = 0; i < mBins.Length; i++)
                {
                    bool targetIsLeftExit = (mTargetPositions[i] == BinPositionExits[0].position);
                    bool targetIsRightExit = (mTargetPositions[i] == BinPositionExits[1].position);



                    //move bin
                    mBins[i].transform.position = Vector3.Lerp(mOriginPositions[i], mTargetPositions[i], positionlerp_Tvalue);


                    if (positionlerp_Tvalue == 1)
                    {
                        //movement finished
                        mIsCurrentlyMoving = false;

                        //check if target is an exit
                        if (targetIsLeftExit || targetIsRightExit)
                        {
                            mHiddenBin = mBins[i];
                            mHiddenBin.SetActive(false);
                        }
                    }
                }

                if (positionlerp_Tvalue == 1)
                {
                    //movement time exceeded, stop moving
                    mIsCurrentlyMoving = false;
                }
            }
        }

        public BinBehaviour[] spawnBins()
        {
            mBins = new GameObject[5];
            mOriginPositions = new Vector3[5];
            mTargetPositions = new Vector3[5];

            mBins[0] = GameObject.Instantiate(BinPrefab1, BinPositions[0].position, Quaternion.identity) as GameObject;
            mBins[1] = GameObject.Instantiate(BinPrefab2, BinPositions[1].position, Quaternion.identity) as GameObject;
            mBins[2] = GameObject.Instantiate(BinPrefab3, BinPositions[2].position, Quaternion.identity) as GameObject;
            mBins[3] = GameObject.Instantiate(BinPrefab4, BinPositions[3].position, Quaternion.identity) as GameObject;

            //the hidden bin
            mBins[4] = GameObject.Instantiate(BinPrefab5, BinPositionExits[0].position, Quaternion.identity) as GameObject;
            mHiddenBin = mBins[4];
            mHiddenBin.SetActive(false);


            for (int i = 0; i < mBins.Length; i++)
            {
                mBins[i].transform.parent = BinParent.transform;

                //set target position to current position since no move has been requested yet
                mTargetPositions[i] = mBins[i].transform.position;
            }
            mHasSpawned = true;

            BinBehaviour[] bins = new BinBehaviour[mBins.Length];
            int j = 0;
            foreach (GameObject go in mBins)
            {
                bins[j++] = go.GetComponent<BinBehaviour>();
            }
            return bins;
        }

        public BinBehaviour[] getBins()
        {
            BinBehaviour[] bins = new BinBehaviour[mBins.Length];
            int j = 0;
            foreach (GameObject go in mBins)
            {
                bins[j++] = go.GetComponent<BinBehaviour>();
            }
            return bins;
        }

        public void moveLeft()
        {
            if (mHasSpawned)
            {
                if (mIsCurrentlyMoving)
                    return;

                mIsCurrentlyMoving = true;
                mTimeSinceMovementStarted = 0;

                for (int i = 0; i < mBins.Length; i++)
                {
                    mOriginPositions[i] = mBins[i].transform.position;

                    //define target position
                    if (mOriginPositions[i] == BinPositions[0].position)
                        mTargetPositions[i] = BinPositionExits[0].position;
                    else if (mOriginPositions[i] == BinPositions[1].position)
                        mTargetPositions[i] = BinPositions[0].position;
                    else if (mOriginPositions[i] == BinPositions[2].position)
                        mTargetPositions[i] = BinPositions[1].position;
                    else if (mOriginPositions[i] == BinPositions[3].position)
                        mTargetPositions[i] = BinPositions[2].position;
                    else if (mBins[i] == mHiddenBin)
                    {
                        mBins[i].SetActive(true);
                        mTargetPositions[i] = BinPositions[BinPositions.Length - 1].position;
                        mOriginPositions[i] = BinPositionExits[1].position;
                    }
                }
            }
        }

        public void moveRight()
        {
            if (mHasSpawned)
            {
                if (mIsCurrentlyMoving)
                    return;

                mIsCurrentlyMoving = true;
                mTimeSinceMovementStarted = 0;

                for (int i = 0; i < mBins.Length; i++)
                {
                    mOriginPositions[i] = mBins[i].transform.position;

                    //define target position
                    if (mOriginPositions[i] == BinPositions[BinPositions.Length - 1].position)
                        mTargetPositions[i] = BinPositionExits[1].position;
                    else if (mOriginPositions[i] == BinPositions[0].position)
                        mTargetPositions[i] = BinPositions[1].position;
                    else if (mOriginPositions[i] == BinPositions[1].position)
                        mTargetPositions[i] = BinPositions[2].position;
                    else if (mOriginPositions[i] == BinPositions[2].position)
                        mTargetPositions[i] = BinPositions[3].position;
                    else if (mBins[i] == mHiddenBin)
                    {
                        mBins[i].SetActive(true);
                        mTargetPositions[i] = BinPositions[0].position;
                        mOriginPositions[i] = BinPositionExits[0].position;
                    }
                }
            }
        }
    }
}