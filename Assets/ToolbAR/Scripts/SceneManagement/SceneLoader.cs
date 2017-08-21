using UnityEngine;
using System.Collections;

namespace ToolbAR.SceneManagement
{
    /// <summary>
    /// This class is a proxy for loading scene asynchronosuly (and maybe additively in the future)
    /// It exists as controller for the loading process, and destroys itself after the scene has been loaded
    /// This gameObject assumes full-control over the GameObject it attaches to.
    /// Destroying this Component/GO will result in cancellation of the load and thus undefined behaviour (as unity has not documented what happens)
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {

        public delegate void OnProgressHandler(SceneLoader loader);

        #region Public Properties

        /// <summary>
        /// The priority of the loading process
        /// See AsyncOperation.priority
        /// </summary>
        public int Priority
        {
            get
            {
                return mInternalOperation.priority;
            }
            set
            {
                mInternalOperation.priority = value;
            }
        }

        /// <summary>
        /// Is true after the first Update of this GO.
        /// </summary>
        public bool HasBegunLoading
        {
            get
            {
                //In special cases, even the first update comes later as the first progress
                return mHasBegunLoading || mInternalOperation.progress > 0f;
            }
        }

        /// <summary>
        /// A wrapped AsyncOperation.isDone
        /// Takes the following bug into account and returns true as soon as it seems like the Scene is ready to be changed
        /// http://forum.unity3d.com/threads/using-allowsceneactivation.166106/
        /// </summary>
        public bool HasFinishedLoading
        {
            get
            {
                return Progress == 1f;
            }
        }
        /// <summary>
        /// A wrapped AsyncOperation.isDone
        /// Takes the following bug into account and returns true as soon as it seems like the Scene is ready to be changed
        /// http://forum.unity3d.com/threads/using-allowsceneactivation.166106/
        /// </summary>
        public bool IsDone
        {
            get
            {
                return HasFinishedLoading;
            }
        }

        public bool IsHaltingSceneActivation
        {
            get
            {
                return !mInternalOperation.allowSceneActivation;
            }
        }

        /// <summary>
        /// A wrapped AsyncOperation.progress
        /// Takes the following bug into account and returns a range where 1.0 is when it seems like the Scene is ready to be changed
        /// http://forum.unity3d.com/threads/using-allowsceneactivation.166106/
        /// </summary>
        public float Progress
        {
            get
            {
                return Mathf.InverseLerp(0f, SceneManager.MAGIC_NUMBER_FINISHED_ASYNC_LOAD, mInternalOperation.progress);
            }
        }

        /// <summary>
        /// Statistic value, return the amount of update cycles this object lived through till now.
        /// This is the amount of update cycles it took to laod the level.
        /// </summary>
        public int StatisticUpdateCycles
        {
            get
            {
                return mUpdateCycleCount;
            }
        }

        /// <summary>
        /// The TargetScene. This is a informational payload, set through createFor
        /// </summary>
        public Scene TargetScene
        {
            get
            {
                return mTargetScene;
            }
        }

        /// <summary>
        /// Mainly useful for the OnProgressHandler, shows if this loader is beeing destroyed now.
        /// </summary>
        public bool IsDeparting
        {
            get
            {
                return mIsDeparting;
            }
        }


        public AsyncOperation InternalOperation
        {
            get
            {
                return mInternalOperation;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a SceneLoader for a specific Load Operation
        /// Called by the SceneManager
        /// </summary>
        /// <param name="TargetScene"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static SceneLoader createFor(Scene TargetScene, AsyncOperation operation, OnProgressHandler progressHandler)
        {
            GameObject go = new GameObject("Scene Loader [" + TargetScene.Name + "]");
            SceneLoader loader = go.AddComponent<SceneLoader>();
            loader.mTargetScene = TargetScene;
            loader.mInternalOperation = operation;
            loader.mInternalOperation.allowSceneActivation = false;
            loader.mProgressHandler = progressHandler;

            return loader;
        }

        #endregion

        #region Private Fields

        private Scene mTargetScene = null;
        private AsyncOperation mInternalOperation = null;
        private OnProgressHandler mProgressHandler = null;

        private bool mHasBegunLoading = false;
        private bool mIsDeparting = false;

        private int mUpdateCycleCount = 0;

        #endregion

        #region Unity Messages/Events

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void OnLevelWasLoaded(int idx)
        {
            if (idx == TargetScene.Index)
            {
                DestroyImmediate(this.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            mUpdateCycleCount++;
            mHasBegunLoading = true;

            if (mProgressHandler != null)
            {
                mProgressHandler(this);
            }

        }

        void OnDestroy()
        {
            mIsDeparting = true;
            if (mProgressHandler != null)
            {
                mProgressHandler(this);
            }
        }

        #endregion
    }
}