using System.Collections.Generic;
using Polycular;
using UnityEngine;
using Vuforia;

namespace EcoGotchi
{
	public class ARMediator : MonoBehaviour
	{
		public Camera _arRenderCamera;
		RenderTexture _arRenderTex;

		Dictionary<string, ImageTargetBehaviour> _nameToGameObject;

		bool _vuforiaActive;
		public bool VuforiaActive
		{
			get { return _vuforiaActive; }
			set
			{
				_vuforiaActive = value;

				if (_vuforiaActive && !CameraDevice.Instance.IsActive())
				{
					VuforiaBehaviour.Instance.enabled = true;
					CameraDevice.Instance.Start();
				}
				else if (!_vuforiaActive && CameraDevice.Instance.IsActive())
				{
					CameraDevice.Instance.Stop();
					VuforiaBehaviour.Instance.enabled = false;
				}
			}
		}


		void Awake()
		{
			Eventbus.Instance.AddListener<CamReadyEvent>((ev) =>
			{
				//RenderTexture.active = _arRenderTex;
				//_arRenderCamera.targetTexture = _arRenderTex;

				//Eventbus.Instance.FireEvent<ARTexReadyEvent>(new ARTexReadyEvent(GetVuforiaFeed()));
			}, this);
		}

		void Start()
		{
			//_arRenderCamera = GetComponentInChildren<Camera>();

			float aspect = (Screen.width < Screen.height) ? ((float)Screen.height / Screen.width) : ((float)Screen.width / Screen.height);
			int rtWidth = Screen.width;
			int rtHeight = Screen.height;

			if (aspect != (16f / 9f) && aspect != (16f / 10f))
			{
				rtWidth = 1080;
				rtHeight = 1920;
			}

			// Initialize a RenderTexture for the Vuforia cam so we dont
			// directly render to screen.
			_arRenderTex = new RenderTexture(rtWidth, rtHeight, 1);

			_nameToGameObject = new Dictionary<string, ImageTargetBehaviour>();
			var trackables = FindObjectsOfType<ImageTargetBehaviour>();

			foreach (var imgTarget in trackables)
			{
				_nameToGameObject.Add(imgTarget.gameObject.name, imgTarget);
			}

			DisableAllImageTargets();
		}

		void OnDestroy()
		{
			Eventbus.Instance.RemoveListener(this);
		}

		public Texture GetVuforiaFeed()
		{
			return _arRenderTex;
		}

		public GameObject GetImageTargetGameObject(string name)
		{
			if (!_nameToGameObject.ContainsKey(name))
				return null;

			return _nameToGameObject[name].gameObject;
		}

		///<summary>Returns wether an ImageTarget with the given name exists.
		///If yes, the ImageTarget's GameObject is set to 'active' state.</summary>
		public bool SetActiveImageTarget(string name, bool active)
		{
			if (!_nameToGameObject.ContainsKey(name))
			{
				return false;
			}

			_nameToGameObject[name].enabled = active;
			// Debug.LogFormat("Marker '{0}' set to be {1}", name, active ? "ACTIVE" : "INACTIVE");

			return true;
		}

		/// <summary>
		/// When a marker is tracked while a scene is loaded additively the image target
		/// has to reach the loaded components which are registered to receive the
		/// trackable found event.
		/// </summary>
		public bool RefreshImageTarget(string name)
		{
			if (!_nameToGameObject.ContainsKey(name) || !_nameToGameObject[name].enabled)
			{
				return false;
			}

			_nameToGameObject[name].enabled = false;
			_nameToGameObject[name].enabled = true;

			return true;
		}

		public void DisableAllImageTargets()
		{
			foreach (var kvp in _nameToGameObject)
			{
				if (kvp.Value.enabled)
					kvp.Value.enabled = false;
			}
		}
	}
}