using System;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;

namespace Polycular.LocationServices
{
	public class AndroidLocationService : MonoBehaviour, ILocationService
	{
		Dictionary<string, Geofence> _fences = new Dictionary<string, Geofence>();

		public string CurrentLocId { get; set; }
		public float UpdateIntervalInSec { get; set; }

		double _distance;
		public double Distance
		{
			get { return Math.Floor(_distance); }
			private set
			{
				if (Math.Floor(value) != Distance)
				{
					_distance = value;
					DistanceChanged(_distance);
				}
			}
		}

		public event Action<double> DistanceChanged;
		public event Action<string> ReachedLocation;

		const string UNITY_PLAYER_ACT_PATH = "com.unity3d.player.UnityPlayer";
		const string PLUGIN_INTERFACE_PATH = "com.polycular.androidnativeplugin.unity.PluginInterface";

		AndroidJavaObject GetUnityPlayerAppContext()
		{
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass(UNITY_PLAYER_ACT_PATH))
			{
				if (unityPlayer != null)
				{
					AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
					return activity.Call<AndroidJavaObject>("getApplicationContext");
				}
				else
				{
					Debug.LogWarning("AndroidNativePlugin::GetUnityPlayerAppCtx" + ": UnityPlayer -> NULL");
					return null;
				}
			}
		}

		AndroidJavaObject GetPluginInterface()
		{
			using (AndroidJavaClass pluginClass = new AndroidJavaClass(PLUGIN_INTERFACE_PATH))
			{
				// Get a PluginInterface instance
				return pluginClass.CallStatic<AndroidJavaObject>("getInstance");
			}
		}

		void SetPluginInterfaceContext(AndroidJavaObject context, AndroidJavaObject pluginInterface)
		{
			// Provide the Unity AndroidPlayer context
			if (context != null && pluginInterface != null)
				pluginInterface.Call("setContext", context);
			else
				Debug.LogWarning("AndroidNativePlugin::SetPluginInterfaceContext" + ": Context AND/OR PluginInterface -> NULL");
		}

		void Start()
		{
			UpdateIntervalInSec = 0.5f;
			InitService();
		}

		public void InitService()
		{
			AndroidJavaObject context = GetUnityPlayerAppContext();
			AndroidJavaObject pluginInterface = GetPluginInterface();

			SetPluginInterfaceContext(context, pluginInterface);

			pluginInterface.Call<int>("initPlugin",
				"CityCaching",
				"Du hast den Ort %s erreicht",
				"Cachy ist auf der Suche nach interessanten Orten!",
				"LocationService",
				"LocationReached",
				1
			);

			Distance = 9999;
			StartLocationService();
		}

		public void AddGeoFence(string identifier, double latitude, double longitude, double radius)
		{
			if (_fences.ContainsKey(identifier))
			{
				return;
			}

			_fences.Add(identifier, new Geofence
			{
				id = identifier,
				latLng = new Vector2((float)longitude, (float)latitude),
				radius = radius
			});

			AndroidJavaObject pluginInterface = GetPluginInterface();
			Debug.Assert(pluginInterface != null);
			pluginInterface.Call<int>("addGeofence", identifier, latitude, longitude, radius);
		}
		public void RemoveGeoFence(string identifier)
		{
			_fences.Remove(identifier);

			AndroidJavaObject pluginInterface = GetPluginInterface();
			Debug.Assert(pluginInterface != null);
			pluginInterface.Call<int>("removeGeofence", identifier);
		}

		public void FireLocationReachedEvent()
		{
			LocationReached(CurrentLocId);
		}

		public double GetCurrentDistance()
		{
			double minDist = 900000;
			Vector2 currentLoc = GetCurrentLocation();

			foreach (var fence in _fences)
			{
				double dist = Math.Max(Haversine.Distance(currentLoc, fence.Value.latLng), 0.0);

				if (dist < minDist)
					minDist = dist;
			}

			return minDist;
		}

		public Vector2 GetCurrentLocation()
		{
			AndroidJavaObject pluginInterface = GetPluginInterface();
			Debug.Assert(pluginInterface != null);
			double lon = pluginInterface.Call<double>("getCurrentLongitude");
			double lat = pluginInterface.Call<double>("getCurrentLatitude");
			return new Vector2((float)lon, (float)lat);
		}

		public void LocationReached(string id)
		{
			if (id.Equals(CurrentLocId))
			{
				_fences.Remove(id);
				ReachedLocation(id);
			}
		}

		public void OpenLocationSettings()
		{
		}

		public void SetUpdates(bool enabled)
		{
			StopCoroutine("UpdateCurrentDistance");

			if (enabled)
				StartCoroutine("UpdateCurrentDistance");
		}

		IEnumerator UpdateCurrentDistance()
		{
			while (true)
			{
				Distance = GetCurrentDistance();
				yield return new WaitForSeconds(UpdateIntervalInSec);
			}
		}

		public void StartLocationService()
		{
			AndroidJavaObject pluginInterface = GetPluginInterface();
			Debug.Assert(pluginInterface != null);
			pluginInterface.Call<int>("startMasterService");
		}

		public void StopLocationService()
		{
			AndroidJavaObject pluginInterface = GetPluginInterface();
			Debug.Assert(pluginInterface != null);
			pluginInterface.Call<int>("stopMasterService");
		}
	}
}
