using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Polycular.LocationServices
{
	public class IOSLocationService : MonoBehaviour, ILocationService
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

		[DllImport("__Internal")]
		static extern void _InitLocationService(
			string notificationTitleText,
			string requestLocationServiceEnabledText,
			string locationReachedFormattedText,
			string alertAcceptText,
			string alertRejectText,
			string bridgeObjectName,
			string bridgeObjectLocationReachedMethodName);

		[DllImport("__Internal")]
		static extern void _OpenLocationSettings();

		[DllImport("__Internal")]
		static extern void _AddGeoFence(string identifier, double latitiude, double longitude, double radius);

		[DllImport("__Internal")]
		static extern void _RemoveGeoFence(string identifier);

		[DllImport("__Internal")]
		static extern double _GetCurrentLongitude();

		[DllImport("__Internal")]
		static extern double _GetCurrentLatitude();

		void Start()
		{
			UpdateIntervalInSec = 0.5f;
			InitService();
		}

		public void InitService()
		{
			_InitLocationService(
				"CityCaching",
				"Damit du die Ortsrätsel lösen kannst schalte bitte deinen Ortsservice wieder ein.",
				"Du hast den Ort %@ erreicht",
				"Ok",
				"Nein",
				"LocationService",
				"LocationReached"
			);
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

			_AddGeoFence(identifier, latitude, longitude, radius);
		}

		public void RemoveGeoFence(string identifier)
		{
			_fences.Remove(identifier);
			_RemoveGeoFence(identifier);
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
			double lon = _GetCurrentLongitude();
			double lat = _GetCurrentLatitude();
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
			_OpenLocationSettings();
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
		}

		public void StopLocationService()
		{
			throw new NotImplementedException();
		}
	}
}