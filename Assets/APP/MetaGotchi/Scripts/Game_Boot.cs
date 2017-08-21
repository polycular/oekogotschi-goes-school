using UnityEngine;
using System.Collections;

using ToolbAR.SceneManagement;
public class Game_Boot : MonoBehaviour {

    public SceneRef SceneToLoadAfterBoot = null;

	// Use this for initialization
	void Start () {
        if (SceneToLoadAfterBoot != null && SceneToLoadAfterBoot.IsValid)
        {
            SceneToLoadAfterBoot.Scene.load();
        }
    }
}
