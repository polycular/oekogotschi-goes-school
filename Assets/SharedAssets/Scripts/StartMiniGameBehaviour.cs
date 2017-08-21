using UnityEngine;
using System.Collections;

using SharedAssets.GameManagement;

namespace SharedAssets
{
    /// <summary>
    /// A hacked helper class, which directly starts a game with the given name
    /// </summary>
    public class StartMiniGameBehaviour : MonoBehaviour
    {
        public string MiniGameName = null;
        // Use this for initialization
        void Start()
        {
            if (MiniGameName != null)
            {
                MiniGameBehaviour minigame = MetaGame.Instance.Behaviour.MiniGames[MiniGameName];
                if (minigame == null)
                    ToolbAR.LogAR.logWarning("Could not Start MiniGame, none found with Name \"" + MiniGameName + "\"", this, this);
                else
                    minigame.startSession();
            }
        }
    }
}