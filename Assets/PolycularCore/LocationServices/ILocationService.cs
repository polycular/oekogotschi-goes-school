using System;
using UnityEngine;

namespace Polycular.LocationServices
{
	public class Geofence
	{
		public string id;
		public Vector2 latLng;
		public double radius;
	}

	public interface ILocationService
	{
		string CurrentLocId { get; set; }

		double Distance { get; }

		float UpdateIntervalInSec { get; set; }

		event Action<double> DistanceChanged;

		event Action<string> ReachedLocation;

		void FireLocationReachedEvent();

		void InitService();

		void OpenLocationSettings();

		void StartLocationService();

		void StopLocationService();

		void AddGeoFence(string identifier, double latitude, double longitude, double radius);

		void RemoveGeoFence(string identifier);

		void LocationReached(string id);

		Vector2 GetCurrentLocation();

		double GetCurrentDistance();

		void SetUpdates(bool enabled);
	}
}
