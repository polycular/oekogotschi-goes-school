using UnityEngine;
using System.Collections;

using ToolbAR.SceneManagement;

namespace SharedAssets
{
    public class PersistOnSceneChangeBehaviour : MonoBehaviour
    {
        public SceneRef Origin;
        /// <summary>
        /// Automatically sets the origin on Awake, if it's null or invalid
        /// </summary>
        public bool AutoSetOriginOnAwake = true;
        /// <summary>
        /// If true, the GameObject protected by this Peristance behaviour will be destroyed, should the SceneManager report to return to the Origin scene
        /// This avoids duplicates easily
        /// Only works when Origin != null
        /// </summary>
        public bool AutoDestroyBeforeOrigin = true;
        /// <summary>
        /// If true, events are also handled while this behaviour or object is disabled
        /// If false, this script my miss scene changes, which means that duplicates may occur even when AutoDestroyBeforeOrigin is true
        /// </summary>
        public bool ListenToEventsWhileDisabled = true;

        void Awake()
        {
            if ((Origin == null || Origin.IsInvalid) && AutoSetOriginOnAwake)
            {
                Origin = SceneRef.fromScene(SceneManager.Instance.CurrentScene);
            }

            if (AutoDestroyBeforeOrigin && Origin != null && Origin.IsValid)
            {
                linkEventListeners();
            }

            DontDestroyOnLoad(this.gameObject);
        }

        void OnEnable()
        {
            linkEventListeners();
        }
        void OnDisable()
        {
            if (!ListenToEventsWhileDisabled)
            {
                removeEventListeners();
            }
        }

        void OnDestroy()
        {
            removeEventListeners();
        }

        public void linkEventListeners()
        {
            if (Origin != null && Origin.IsValid)
            {
                Origin.Scene.beforeSceneChangeTo -= beforeSceneChangeTo;
                Origin.Scene.beforeSceneChangeTo += beforeSceneChangeTo;
            }
        }

        void removeEventListeners()
        {
            if (Origin != null && Origin.IsValid)
            {
                Origin.Scene.beforeSceneChangeTo -= beforeSceneChangeTo;
            }
        }

        void beforeSceneChangeTo(Scene oldScene, Scene newScene)
        {
            if (AutoDestroyBeforeOrigin && Origin != null && newScene == Origin.Scene)
            {
                Destroy(this.gameObject);
            }
        }
    }
}