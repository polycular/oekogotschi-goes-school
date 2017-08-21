using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

    
    public string LoadingScreenScene;

    
    private static GameObject instance;

    public static SceneLoader Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.Find("SceneLoader");
            }

            if (!instance)
            {
                instance = Instantiate(((GameObject)Resources.Load("SceneLoader")));
                
            }

            
            return instance.GetComponent<SceneLoader>();
        }
        



    }

   

    bool sceneisloading = false;
    public void ChangeScene(int SceneID)
    {


        //loadscreen.allowSceneActivation = true;
        if (!sceneisloading)
        {
            sceneisloading = true;

            StartCoroutine(StartLoadingAdditive(LoadingScreenScene));

            StartCoroutine(StartLoading(SceneID));

           
        }

        
        



    }

    
   

   

    private IEnumerator StartLoading(int LevelId)
    {        
        AsyncOperation async = Application.LoadLevelAsync(LevelId);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {

            Debug.Log("Progress: " + (async.progress * 100) + "%");
            yield return null;
        }
        async.allowSceneActivation = true;
       
    }

    private IEnumerator StartLoadingAdditive(int LevelId)
    {

        AsyncOperation async = Application.LoadLevelAdditiveAsync(LevelId);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {

            Debug.Log("Progress: " + (async.progress * 100) + "%");
            yield return null;
        }
       
        async.allowSceneActivation = true;
    }

    private IEnumerator StartLoadingAdditive(string LevelName)
    {

        AsyncOperation async = Application.LoadLevelAdditiveAsync(LevelName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {

            Debug.Log("Progress: " + (async.progress * 100) + "%");
            yield return null;
        }

        async.allowSceneActivation = true;
    }

}
