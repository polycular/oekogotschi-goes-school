using UnityEngine;
using System.Collections;

using ToolbAR.SceneManagement;

namespace SharedAssets.GameManagement
{
    /// <summary>
    /// Represents a Session of a MiniGame.
    /// Objects in the Sub-Hierarchy will only be persistent until the SceneManager is requested to return to the MetaGame.MainScene
    /// 
    /// </summary>
    public class MiniGameSessionBehaviour : MonoBehaviour
    {
        /// <summary>
        /// True if the Session was completed successfully
        /// </summary>
        public bool IsSuccessful = false;
        /// <summary>
        /// Score of this Session
        /// </summary>
        public int Score = 0;

        /// <summary>
        /// Used for Editor Only, to link whihc MiniGame this Session belongs to, when Play-Testing
        /// </summary>
        public MiniGameBehaviour LinkedMiniGamePrefab;
        /// <summary>
        /// This is the -actual- MiniGame this GO is a Session of.
        /// Set at runtime, this can be accessed over the getter
        /// </summary>
        MiniGameBehaviour mMiniGame;

        private bool mHasStartedLoading = false;

        public MiniGameBehaviour MiniGame
        {
            get
            {
                return mMiniGame;
            }
        }


        void Awake()
        {
#if UNITY_EDITOR
            if (MetaGame.Instance.Behaviour.CurrentMiniGame == null)
            {
                if (LinkedMiniGamePrefab == null)
                {
                    Debug.LogError("MiniGameSession was misconfigured for playtest:  No LinkedMiniGamePrefab given");
                    this.gameObject.SetActive(false);
                    return;
                }
                MiniGameBehaviour minigame = MetaGame.Instance.Behaviour.MiniGames[LinkedMiniGamePrefab.name];
                if (minigame == null)
                {
                    Debug.LogError("Session could not attach to MiniGame of prefab \"" + LinkedMiniGamePrefab.name + "\" because it was not found", LinkedMiniGamePrefab);
                    this.gameObject.SetActive(false);
                    return;
                }
                MetaGame.Instance.Behaviour.explicitlySetCurrentMiniGame(minigame);
            }
#endif
            //Only one Session can exist at a given time
            if (GameObject.FindObjectsOfType<MiniGameSessionBehaviour>().Length > 1)
            {
                //There already is a session somewhere
                Destroy(this.gameObject);
                return;
            }

            //Configure minimum persistence for any type of Session
            var persistence = gameObject.AddComponent<PersistOnSceneChangeBehaviour>();
            persistence.ListenToEventsWhileDisabled = true;
            persistence.AutoDestroyBeforeOrigin = true;
            persistence.Origin = SceneRef.fromScene(MetaGame.Instance.Behaviour.MainScene);
            persistence.linkEventListeners();

            mMiniGame = MetaGame.Instance.Behaviour.CurrentMiniGame;
            gameObject.name = mMiniGame.gameObject.name + "Session";
            mMiniGame.CurrentSession = this;
        }

        void OnDestroy()
        {
            if (mMiniGame != null)
            {
                mMiniGame.dismissSession(this);
            }
        }

        void Start()
        {
        }

        void Update()
        {
            if (Application.loadedLevel == MetaGame.Instance.Behaviour.IntermediateScene.Index)
            {
                if (MetaGame.Instance.Behaviour.CurrentMiniGame != null)
                {
                    if (MetaGame.Instance.Behaviour.CurrentMiniGame.CurrentSession == this)
                    {
                        if (!mHasStartedLoading)
                        {
                            mHasStartedLoading = true;
                            LinkedMiniGamePrefab.EntryScene.loadAsync();
                        }
                    }
                }
            }
        }
    }
}
