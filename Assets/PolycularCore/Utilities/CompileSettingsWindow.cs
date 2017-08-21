#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Polycular.Utilities
{
	public class CompileSettingsWindow : EditorWindow
	{
		[MenuItem("Window/Compile")]
		private static void NewWindow()
		{
			CompileSettingsWindow ed = GetWindow<CompileSettingsWindow>();
			ed.Init();
		}

		bool autoRecompile;
		bool AutoRecompile
		{
			get { return autoRecompile; }

			set
			{
				autoRecompile = value;
				EditorPrefs.SetBool("kAutoRefresh", autoRecompile);
			}
		}

		public void Init()
		{
			AutoRecompile = EditorPrefs.GetBool("kAutoRefresh");
		}

		void OnDestroy()
		{
			AutoRecompile = true;
		}

		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Auto Recompile");
			AutoRecompile = EditorGUILayout.Toggle(AutoRecompile);
			GUILayout.EndHorizontal();

			GUILayout.Space(20);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Compile"))
			{
				AssetDatabase.Refresh();
			}

			GUILayout.EndHorizontal();
		}
	}
}
#endif
