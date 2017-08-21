using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CycleGotchi
{
    [CustomEditor(typeof(TileBehaviour), true)]
    public class TileBehaviourEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            if (this.target.GetType() != typeof(TileBehaviour))
                return;
            TileBehaviour target = (TileBehaviour)this.target;


            DrawDefaultInspector();

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(target.IsGood);
            if (GUILayout.Button("Turn Good"))
            {
                target.turnGood();
            }
            EditorGUI.EndDisabledGroup();

            /*
            if (GUILayout.Button("Test Select North"))
            {
                target.ParentGrid.recollectGrid();
                foreach (var tile in target.ParentGrid.selectTileArea(target, TileBehaviour.Edge.NORTH))
                {
                    Debug.Log(tile);
                }
            }
            if (GUILayout.Button("Test Select East"))
            {
                target.ParentGrid.recollectGrid();
                foreach (var tile in target.ParentGrid.selectTileArea(target, TileBehaviour.Edge.EAST))
                {
                    Debug.Log(tile);
                }
            }
            if (GUILayout.Button("Test Select South"))
            {
                target.ParentGrid.recollectGrid();
                foreach (var tile in target.ParentGrid.selectTileArea(target, TileBehaviour.Edge.SOUTH))
                {
                    Debug.Log(tile);
                }
            }
            if (GUILayout.Button("Test Select West"))
            {
                target.ParentGrid.recollectGrid();
                foreach (var tile in target.ParentGrid.selectTileArea(target, TileBehaviour.Edge.WEST))
                {
                    Debug.Log(tile);
                }
            }
             * */
        }
    }
}