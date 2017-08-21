using UnityEngine;

namespace Polycular.Utilities
{
	/// <summary>
	/// Supports 'Standalone' | 'Unity Editor' | 'Android' | 'iOS'
	/// </summary>
	public static class StreamingAssetsUtil
	{
		public static string GetPlatformPath(string relPath)
		{
#if UNITY_ANDROID && !UNITY_STANDALONE && !UNITY_EDITOR
			return "jar:file:///" + Application.dataPath + "!/assets/" + relPath;
#elif UNITY_IOS && !UNITY__STANDALONE && !UNITY_EDITOR
			return "file:///" + Application.dataPath + "/Raw/" + relPath;
#elif UNITY_STANDALONE || UNITY_EDITOR
			return "file:///" + Application.streamingAssetsPath + "/" + relPath;
#endif
		}
	}
}