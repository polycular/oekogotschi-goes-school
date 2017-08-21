using System;
using System.IO;
using Flurl;
using UnityEngine;

namespace Polycular.ResourceProvider
{
    public class DiskOpResult
    {
        public bool Success { get; set; }

        public Exception Error { get; set; }

        public string ReadContent { get; set; }
    }

	public enum StorageType
	{
		Local,
		Persistant
	}

	public class DiskResourceProvider : IDisposable
	{
		string basePath;
		
		public DiskResourceProvider()
		{
			basePath = Application.persistentDataPath;
		}

		public DiskResourceProvider(StorageType disktype)
		{
			if (disktype == StorageType.Local)
				basePath = Application.dataPath;
			else
				basePath = Application.persistentDataPath;
		}

		public DiskOpResult Read(string uri)
		{
			string filePath = GetUnityPath(uri);
            var result = new DiskOpResult();

            try
			{
				result.ReadContent = File.ReadAllText(filePath);
                result.Success = true;
			}
			catch (Exception ex)
			{
                Debug.LogWarning(ex);

                result.ReadContent = null;
                result.Success = false;
                result.Error = ex;
			}
            return result;
		}

		public DiskOpResult Write(string uri, string content)
		{
			string filePath = GetUnityPath(uri);
			string dirPath = Path.GetDirectoryName(filePath);
            var result = new DiskOpResult();

			// create directories unless they already exist
			Directory.CreateDirectory(dirPath);

			try
			{
				File.WriteAllText(filePath, content);
                result.Success = true;
			}
			catch (Exception ex)
			{
                Debug.LogWarning(ex);
                result.Success = false;
                result.Error = ex;
            }

            return result;
		}

		public DiskOpResult Delete(string uri)
		{
			string path = GetUnityPath(uri);
			string fileName = Path.GetFileName(path);
			bool isFile = (fileName != string.Empty && fileName != null);
            var result = new DiskOpResult();

            if (isFile)
			{
				if (File.Exists(path))
				{
					File.Delete(path);
                    result.Success = true;
				}
				else
				{
                    result.Success = false;
					result.Error = new FileNotFoundException();
				}
			}
			else
			{
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);
                    result.Success = true;
				}
				else
				{
					result.Error = new DirectoryNotFoundException();
                    result.Success = false;
				}
			}

			return result;
		}

		string GetUnityPath(string path)
		{
			string fullPath = Path.Combine(basePath, path);
			return fullPath;
		}

		public void Dispose()
		{
		}
	}
}