using System.IO;
using Newtonsoft.Json.Linq;
using Polycular.ResourceProvider;
using UnityEngine;

namespace Polycular.Persistance
{
	static class Storage
	{
		static Storage()
		{
			basePath = GetConfigValue("lastUser");
		}

		// = user id; the root folder for each user containing all the user's data
		static string basePath;
		public static string BasePath
		{
			get
			{
				if (basePath == null)
				{
					// check if the last user is present in the config file
					var lastUserId = GetConfigValue("lastUser");

					if (lastUserId != null)
						basePath = lastUserId;
					else
					{
						basePath = "";
						Debug.LogWarning("No LastUser was found or set, Storage will read/write from persistant storage root.");
					}
				}

				return basePath;
			}
			set
			{
				basePath = value.Trim();
			}
		}

		static DiskResourceProvider drp = new DiskResourceProvider();
		static string configFilePath = "config.json";

		// loads a value from the global config file if present
		public static string GetConfigValue(string key)
		{
			var config = LoadJsonConfig();
			return config == null ? null : (string)config[key];
		}

		public static void SetConfigValue(string key, string value)
		{
			var config = LoadJsonConfig() ?? new JObject();

			// check if key is already present
			if (string.IsNullOrEmpty((string)config[key]))
			{
				config.Add(key, value);
			}
			else
				config[key] = value;

			SaveJsonConfig(config);
		}

		public static void RemoveConfigValue(string key)
		{
			var config = LoadJsonConfig();

			if (config == null)
				return;

			config.Remove(key);

			SaveJsonConfig(config);
		}

		static JObject LoadJsonConfig()
		{
			var result = drp.Read(new Flurl.Url(configFilePath));

			if (!result.Success)
			{
				Debug.LogWarning("Storage: Global config file couldn't be read");
				return null;
			}

			if (string.IsNullOrEmpty(result.ReadContent))
			{
				Debug.LogWarning("Storage: Global config file is empty");
				return null;
			}

			else return JObject.Parse(result.ReadContent);
		}

		static void SaveJsonConfig(JObject config)
		{
			var result = drp.Write(new Flurl.Url(configFilePath), config.ToString());

			if (!result.Success)
			{
				Debug.LogWarning("Storage: An error occurred while writing the global config file");
			}
		}


		public static void Save(IPersistant obj)
		{
			if (BasePath == null)
				return;

			var serialized = obj.Serialize();
			var savePath = obj.SavePath;

			if (savePath == null)
				return;

			drp.Write(Path.Combine(BasePath, savePath), serialized);
		}

		public static void Load(IPersistant obj)
		{
			if (BasePath == null)
				return;

			var savePath = obj.SavePath;

			if (savePath == null)
				return;

			var result = drp.Read(Path.Combine(BasePath, savePath));

			if (!result.Success)
			{
				return;
			}

			obj.Deserialize(result.ReadContent);
		}
	}
}
