using UnityEngine;
using System.Collections;

using ToolbAR.SceneManagement;

namespace SharedAssets.GameManagement
{
    /// <summary>
    /// Represents a MiniGame, or more precisely, its configuration.
    /// To customize the Behaviour of MiniGames before they are started,
    /// attach oher Behaviours to the MiniGame prefab with this Behaviour.
    /// 
    /// Attention: For MiniGames to be Recognized and loaded in MetaGame,
    /// they must have a Prefab with this Behaviour in "Resources/Prefabs/MiniGames/"
    /// 
    /// The EntryScene is the actual starting point for any game. MiniGames without Configured Entry Scenes will not be able to start.
    /// Upon Start, a MiniGameSession Behaviour will be created, and the EntryScene will be loaded
    /// 
    /// </summary>
    public class MiniGameBehaviour : MonoBehaviour
    {
        public Scene EntryScene
        {
            get
            {
                return mEntryScene;
            }
        }
        public MiniGameSessionBehaviour CurrentSession
        {
            get
            {
                return mCurrentSession;
            }
            set
            {
                if (value == null)
                {
                    mCurrentSession = null;
                }
                else if (mCurrentSession == null)
                    mCurrentSession = value;
                else
                    ToolbAR.LogAR.logWarning("Tried to overwrite MiniGameBehaviour::CurrentSession with " + value, this, this);
            }
        }

        /// <summary>
        /// The Highest Score of this Trial till now
        /// </summary>
        public int Highscore = 0;

        /// <summary>
        /// Indicates if the trial was played successfully
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// Tries to create a new session and loads the EntryScene
        /// </summary>
        /// <returns>the new session, null if no new session was created</returns>
        public MiniGameSessionBehaviour startSession(bool loadScene = true)
        {
            if (mCurrentSession != null && MetaGame.Instance.Behaviour.CurrentMiniGame != null)
                return null;

            MetaGame.Instance.Behaviour.explicitlySetCurrentMiniGame(this);

            if (loadScene)
            {
                MetaGame.Instance.Behaviour.IntermediateScene.load();
            }

            GameObject session = null;
            if (mSessionPrefab != null)
            {
                session = (Instantiate(mSessionPrefab.gameObject) as GameObject);
            }
            else
            {
                session = new GameObject();
            }
            if (session.GetComponent<MiniGameSessionBehaviour>() == null)
                session.AddComponent<MiniGameSessionBehaviour>();

            mCurrentSession = session.GetComponent<MiniGameSessionBehaviour>();

            return mCurrentSession;
        }

        /// <summary>
        /// Returns to the MetaGameMain Scene, which also destroys the session
        /// </summary>
        public void quitSession()
        {
            dismissSession(mCurrentSession);
            MetaGame.Instance.Behaviour.IntermediateScene.load();
        }

        /// <summary>
        /// Dismisses the session if it really is the current session
        /// Called by quitSession and Session.OnDestroy()
        /// </summary>
        /// <param name="session"></param>
        public void dismissSession(MiniGameSessionBehaviour session)
        {
            if (session != null && session == mCurrentSession)
            {
                this.Successful = session.IsSuccessful;

                //Here we dismantle the session, take over scores etc.
                if (session.IsSuccessful)
                {
                    //Take over the score
                    this.Highscore = Mathf.Max(session.Score, this.Highscore);
                    //TODO: Send the Score to the Server/Network.

                }
                //Set current sesion to null
                mCurrentSession = null;
            }
        }


        [SerializeField]
        private SceneRef mEntryScene = null;
        [SerializeField]
        /// <summary>
        /// If not null, this prefab will be used as Base to instantiate the MiniGameSession
        /// TODO: Implement
        /// </summary>
        private MiniGameSessionBehaviour mSessionPrefab = null;

        private MiniGameSessionBehaviour mCurrentSession = null;

    }
}