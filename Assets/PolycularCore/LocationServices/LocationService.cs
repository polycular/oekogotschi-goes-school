using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Polycular.LocationServices
{
	public class LocationService : MonoBehaviour, ILocationService
	{
		public string CurrentLocId { get; set; }
		public float UpdateIntervalInSec { get; set; }

		Dictionary<string, Geofence> _fences = new Dictionary<string, Geofence>();
		
		public event Action<double> DistanceChanged;
		public event Action<string> ReachedLocation;

		double _distance = 0.0;
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
		
		public void FireLocationReachedEvent()
		{
			LocationReached(CurrentLocId);
		}

		void Start()
		{
			InitService();
		}

		public void InitService()
		{
			/*
			 	Strings represent title and message used by the foreground service
			
				* string Notification Title
				* string Notification Text
				* string Service Notification Text (Title is the same as for Notifications) - obsolete
				* string bridgeObjectName
				* string bridgeObjectLocationReachedMethodName
				* int Location update interval in seconds
			*/
		}

		public void OpenLocationSettings() {}

		public void StartLocationService() {}
		
		public void StopLocationService() {}

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
		}

		public void RemoveGeoFence(string identifier) {}

		public void LocationReached(string id)
		{
			if (id.Equals(CurrentLocId))
			{
				_fences.Remove(id);
				ReachedLocation(id);
			}
		}

		public Vector2 GetCurrentLocation()
		{
			return Vector2.zero;
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
	}
}