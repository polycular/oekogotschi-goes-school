using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SharedAssets
{

	public class EggTimerBehaviourEditor : Editor
	{ }
	//[CustomEditor(typeof(EggTimerBehaviour), true)]
	//public class EggTimerBehaviourEditor : Editor
	//{
	//    override public void OnInspectorGUI()
	//    {
	//        EggTimerBehaviour target = (EggTimerBehaviour)this.target;


	//        DrawDefaultInspector();

	//        //EditorGUI.BeginChangeCheck();
	//        //target.TimeLimit = EditorGUILayout.FloatField("Set TimeLimit Seconds", target.TimeLimit);
	//        //if (EditorGUI.EndChangeCheck())
	//        //{
	//        //    EditorUtility.SetDirty(target);
	//        //}


	//        GUIStyle countDownStyle = new GUIStyle(GUI.skin.textField);
	//        countDownStyle.alignment = TextAnchor.MiddleCenter;
	//        GUIStyle countDownTitleStyle = new GUIStyle(GUI.skin.label);
	//        countDownTitleStyle.alignment = TextAnchor.MiddleCenter;

	//        EditorGUILayout.Separator();
	//        EditorGUILayout.LabelField("Countdown", countDownTitleStyle);
	//        EditorGUI.BeginDisabledGroup(true);
	//        EditorGUILayout.LabelField(target.CountdownStringP1, countDownStyle);
	//        EditorGUILayout.Separator();
	//        EditorGUILayout.Slider("Remaining Seconds", target.RemainingSeconds, 0f, target.TimeLimit);
	//        EditorGUI.EndDisabledGroup();


	//        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
	//        if (GUILayout.Button("Restart"))
	//        {
	//            target.restart();
	//        }
	//        EditorGUI.EndDisabledGroup();

	//        //Repaint continiously in playmode
	//        if (Application.isPlaying)
	//        {
	//            Repaint();
	//        }

	//    }
	//}
}
