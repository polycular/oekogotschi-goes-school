using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SharedAssets.GameManagement
{
    [CustomEditor(typeof(MiniGameBehaviour), true)]
    public class MiniGameBehaviourEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            if (this.target.GetType() != typeof(MiniGameBehaviour))
                return;
            MiniGameBehaviour target = (MiniGameBehaviour)this.target;
            DrawDefaultInspector();
            if (GUILayout.Button("Start Session"))
            {
                target.startSession();
            }
        }
    }
}
