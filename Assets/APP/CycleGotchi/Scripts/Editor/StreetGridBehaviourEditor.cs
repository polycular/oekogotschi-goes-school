using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CycleGotchi
{
    [CustomEditor(typeof(StreetGridBehaviour), true)]
    public class StreetGridBehaviourEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            if (this.target.GetType() != typeof(StreetGridBehaviour))
                return;
            StreetGridBehaviour target = (StreetGridBehaviour)this.target;


            DrawDefaultInspector();

            EditorGUILayout.Separator();

            if (GUILayout.Button("Recreate Grid"))
            {
                target.recreateGrid();
            }
        }
    }
}