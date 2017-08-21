using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ToolbAR.SceneManagement
{
    [JsonObject(MemberSerialization.OptIn)]
    /// <summary>
    /// The new SceneManager.
    /// On build and play, it automatically collects all Scenes in the BuildSettings, and serializes itself to a json file
    /// At Runtime, it is a singleton that deserializes this json file, and allows Access to Infromation of which scenes are present and which are not.
    /// Also it provides a common API to switch Scenes, which fires events even before the scene is changed, and thus can infrom Persistance Behaviours of upcoming possible destroyal.
    /// </summary>
    public class SceneManager
    {
        public const string RELATIVE_FILE_PATH = "SceneManagement/SceneManager.json"; //Relative to streamingassets dir

        /// <summary>
        /// VBalue at which an async loading is actually complete
        /// See http://forum.unity3d.com/threads/using-allowsceneactivation.166106/
        /// </summary>
        public const float MAGIC_NUMBER_FINISHED_ASYNC_LOAD = 0.9f;

        #region Public Events

        public delegate void BeforeSceneChangeHandler(Scene oldScene, Scene newScene);
        public delegate void AfterSceneChangeHandler(Scene oldScene, Scene newScene);

        /// <summary>
        /// Fired when a Scene Change was requested, but before anything was changed
        /// </summary>
        public event BeforeSceneChangeHandler beforeSceneChange;

        /// <summary>
        /// Fired when a Scene Change was requested, after the CurrentScene was changed and the new Scene was requested to load.
        /// Note that Unity does not ensure that the new scene has loaded yet
        /// 
        /// KNOWN ISSUE: If using asynchronous loading, this will be fired at the same late time as MonoBehaviour.OnLevelWasLoaded. No way to fix that.
        /// </summary>
        public event AfterSceneChangeHandler afterSceneChange;

        #endregion

        #region Public Properties

        public Scene[] Scenes
        {
            get
            {
                return mScenesOrderedByIdx;
            }
        }

        /// <summary>
        /// The current Scene.
        /// This is only Up2Date if the SceneManager API is used to change scenes (including this property, which relays to changeScene(Scene))
        /// </summary>
        public Scene CurrentScene
        {
            get
            {
                return mCurrentScene;
            }
            set
            {
                changeScene(value);
            }
        }

        /// <summary>
        /// If an async process is running,
        /// this gives access to the current loader
        /// </summary>
        public SceneLoader CurrentLoader
        {
            get
            {
                return mCurrentLoader;
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Returns a Scene by its (Build) Index (Also known as Level id)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Scene this[int index]
        {
            get { return getSceneByIndex(index); }
        }
        public Scene this[string path]
        {
            get { return getSceneByPath(path); }
        }

        #endregion 

        #region Public Methods

        #region Scene Change API

        /// <summary>
        /// The SceneManager overwrites the CurrentScene with the actually loaded scenes.
        /// No events will be fired whatsoever
        /// </summary>
        public void forceSetCurrentScene()
        {
            Scene scene = System.Array.Find(mInstance.getAllScenesByName(Application.loadedLevelName), element => element.Index == Application.loadedLevel);
            if (scene == null)
            {
                string error = "SceneManager: CurrentScene \"" + Application.loadedLevelName + "\" is outside of build, expect problems if trying to access the scene";
#if UNITY_EDITOR
                Debug.Log(error);
#else
                    Debug.LogError(error);
#endif
            }
            else
            {
                mCurrentScene = scene;
            }
        }

        public Scene changeScene(Scene target)
        {
            if (target == null)
            {
                Debug.LogError("Could not changeScene, as no Scene was found (scene was null)");
                return null;
            }

            Scene oldScene = mCurrentScene;
            emitBeforeSceneChangeEvent(oldScene, target);
            mCurrentScene = target;
            Application.LoadLevel(target.Index);
            emitAfterSceneChangeEvent(oldScene, target);
            return mCurrentScene;
        }

        public SceneLoader changeSceneAsync(Scene target)
        {
            if (target == null)
            {
                Debug.LogError("Could not changeSceneAsync, as no Scene was found (scene was null)");
                return null;
            }

            if(mCurrentLoader != null)
            {
                Debug.LogError("Could not changeSceneAsync, as changing is already in Progress with " + mCurrentLoader);
                return null;
            }

            mCurrentLoader = SceneLoader.createFor(target, Application.LoadLevelAsync(target.Index), this.onAsyncLoadProgress);
            return mCurrentLoader;
        }

        public Scene changeScene(string targetPath)
        {
            Scene scene = getSceneByPath(targetPath);
            if (scene == null)
            {
                Debug.LogError("Could not changeScene by Path, as no Scene was found at Path \"" + targetPath + "\"");
                return null;
            }
            return changeScene(scene);
        }

        public SceneLoader changeSceneAsync(string targetPath)
        {
            Scene scene = getSceneByPath(targetPath);
            if (scene == null)
            {
                Debug.LogError("Could not changeSceneAsync by Path, as no Scene was found at Path \"" + targetPath + "\"");
                return null;
            }
            return changeSceneAsync(scene);
        }

        public Scene changeScene(int targetIdx)
        {
            Scene scene = getSceneByIndex(targetIdx);
            if (scene == null)
            {
                Debug.LogError("Could not changeScene by Index, as no Scene was found with Index \"" + targetIdx + "\"");
                return null;
            }
            return changeScene(scene);
        }

        public SceneLoader changeSceneAsync(int targetIdx)
        {
            Scene scene = getSceneByIndex(targetIdx);
            if (scene == null)
            {
                Debug.LogError("Could not changeSceneAsync by Index, as no Scene was found with Index \"" + targetIdx + "\"");
                return null;
            }
            return changeSceneAsync(scene);
        }

        #endregion

        #region Scene Getters

        public Scene getSceneByIndex(int index)
        {
            return System.Array.Find(mScenesOrderedByIdx, element => element.Index == index);
        }
        public Scene getSceneByPath(string path)
        {
            if (!path.EndsWith(".unity"))
                path += ".unity";
            Scene scene;
            if (mScenes.TryGetValue(path, out scene))
            {
                return scene;
            }
            else
            {
                return null;
            }
        }
        public Scene getFirstSceneByName(string name)
        {
            return System.Array.Find(mScenesOrderedByIdx, element => element.Name == name);
        }
        public Scene[] getAllScenesByName(string name)
        {
            return System.Array.FindAll(mScenesOrderedByIdx, element => element.Name == name);
        }
        public Scene getSceneByGUID(string guid)
        {
            return System.Array.Find(mScenesOrderedByIdx, element => element.GUID == guid);
        }

        #endregion

        #endregion

        #region Static Public Methods

        static public string getFilePath()
        {
            return Application.streamingAssetsPath + "/" + RELATIVE_FILE_PATH;
        }

        #endregion

        #region Private Methods

        private void onAsyncLoadProgress(SceneLoader loader)
        {
            if (loader != null && loader == mCurrentLoader)
            {
                if (loader.IsDone && loader.IsHaltingSceneActivation)
                {
                    emitBeforeSceneChangeEvent(mCurrentScene, loader.TargetScene);
                    loader.InternalOperation.allowSceneActivation = true;
                }
                if (loader.IsDone && loader.IsDeparting)
                {
                    Scene oldScene = mCurrentScene;
                    mCurrentScene = loader.TargetScene;

                    mCurrentLoader = null;

                    emitAfterSceneChangeEvent(oldScene, loader.TargetScene);

                }
            }
            else
            {
                Debug.LogWarning("There seem to be unregistered SceneLoaders. One of them is " + loader);
            }
        }

        private void emitBeforeSceneChangeEvent(Scene oldScene, Scene newScene)
        {
            if (mCurrentScene != null)
                mCurrentScene.emitBeforeSceneLeaveEvent(oldScene, newScene);
            if (newScene != null)
                newScene.emitBeforeSceneChangeToEvent(oldScene, newScene);
            if (beforeSceneChange != null)
            {
                beforeSceneChange(oldScene, newScene);
            }
        }
        private void emitAfterSceneChangeEvent(Scene oldScene, Scene newScene)
        {
            if (afterSceneChange != null)
            {
                afterSceneChange(oldScene, newScene);
            }
            if (mCurrentScene != null)
                mCurrentScene.emitAfterSceneChangeToEvent(oldScene, newScene);
        }

        #endregion

        #region Private Fields

        [JsonProperty]
        Dictionary<string, Scene> mScenes = new Dictionary<string, Scene>();
        /// <summary>
        /// Runtime generated list on deserialization, an array of scenes, sorted by their Index.
        /// </summary>
        Scene[] mScenesOrderedByIdx;


        /// <summary>
        /// The current Scene.
        /// This is only Up2Date if the SceneManager API is used to change scenes
        /// </summary>
        Scene mCurrentScene = null;

        SceneLoader mCurrentLoader = null;

        #endregion

        #region Loading and Saving

#if UNITY_EDITOR

        /// <summary>
        /// Creates a new SceneManager and serializes it directly.
        /// </summary>
        /// <param name="scenes">A List of Scenes. The Scenes path has to be relative to the Assets/ directory</param>
        /// <param name="final">If true, the serilization will try to minimize the serialized file</param>
        static public void saveToStreamingAssets(Scene[] scenes, bool final)
        {
            SceneManager manager = new SceneManager();

            foreach (Scene scene in scenes)
            {
                manager.mScenes.Add(scene.Path, scene);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(getFilePath()));
            string json = JsonConvert.SerializeObject(manager, ((final)?Formatting.None : Formatting.Indented));
            File.WriteAllText(getFilePath(), json);
        }
#endif

        static protected SceneManager loadFromStreamingAssets()
        {
            string filePath = getFilePath();
            string result = "";
            if (filePath.Contains("://"))
            {
                WWW www = new WWW(filePath);
                while (!www.isDone)
                {

                }
                if (www.error != null && www.error != "")
                {
                    Debug.LogError("ToolbAR.SceneManagement.SceneManager::loadFromStreamingAssets() failed \n" + www.error);
                    return null;
                }
                result = www.text;
            }
            else
            {
                try
                {
                    result = System.IO.File.ReadAllText(filePath);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("ToolbAR.SceneManagement.SceneManager::loadFromStreamingAssets() failed \n" + e.Message);
                    return null;
                }
            }

            SceneManager manager = JsonConvert.DeserializeObject<SceneManager>(result);

            //Build ScenesOrderedByIdx
            manager.mScenesOrderedByIdx = new Scene[manager.mScenes.Count];
            manager.mScenes.Values.CopyTo(manager.mScenesOrderedByIdx, 0);
            System.Array.Sort(manager.mScenesOrderedByIdx,
                delegate(Scene a, Scene b)
                {
                    return a.Index.CompareTo(b.Index);
                });

            return manager;
        }

        #endregion

        #region Singleton
        static private SceneManager mInstance = null;
        static public SceneManager Instance
        {
            get
            {
                ensureInstance();
                return mInstance;
            }
        }
        static public void ensureInstance()
        {
            if (!HasInstance)
            {
                mInstance = loadFromStreamingAssets();
                mInstance.forceSetCurrentScene();
            }
        }
        static public bool HasInstance
        {
            get
            {
                return (mInstance != null);
            }
        }
        [JsonConstructor]
        private SceneManager()
        {
        }
        #endregion
    }
}