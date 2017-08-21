using UnityEngine;
using UnityEditor;
using System.Collections;


namespace SharedAssets
{
    class PlayOptions
    {
        [MenuItem("Edit/Play-Stop from Game_Boot %0")]
        public static void PlayFromBootstrap()
        {
            if (EditorApplication.isPlaying == true)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorApplication.SaveCurrentSceneIfUserWantsTo();
            EditorApplication.OpenScene("Assets/MetaGotchi/Scenes/Game_Boot.unity");
            EditorApplication.isPlaying = true;
        }
    }
}