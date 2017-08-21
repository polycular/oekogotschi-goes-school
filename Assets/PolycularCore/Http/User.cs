using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Polycular.Persistance;

namespace Http
{
	public class User : IPersistant
	{
		public enum AuthenticationType
		{
			NATIVE,
			FACEBOOK
		}

		public AuthenticationType AuthType { get; set; }

		public string Username { get; set; }

    	public string Email { get; set; }
	
    	public string Password { get; set; }

		string id;
		public string Id {
			get { return id; }
			set
			{
				id = value;
				Storage.BasePath = id;
				Storage.SetConfigValue("lastUser", id);
			}
		}

		public string AccessToken { get; set; }

        public DateTime AccessExpiry { get; set; }

        public string RefreshToken { get; set; }
        
		public DateTime RefreshExpiry { get; set; }

		public bool IsTester { get; set; }

		string savePath = "user.json";
		public string SavePath
		{
			get { return savePath; }
		}

		public bool IsLoaded
		{
			get { return (AccessToken != null && RefreshToken != null); }
            private set { }
		}

		public void Deserialize(string serialObject)
		{
			if (string.IsNullOrEmpty(serialObject))
				return;

            var userInfo = JsonConvert.DeserializeObject<User>(serialObject);

            if (userInfo != null)
            {
                Email = userInfo.Email;
                Id = userInfo.Id;
                AccessToken = userInfo.AccessToken;
                AccessExpiry = userInfo.AccessExpiry;
                RefreshToken = userInfo.RefreshToken;
                RefreshExpiry = userInfo.RefreshExpiry;
            }
        }

		public string Serialize() 
		{
            if (string.IsNullOrEmpty(Id))
                return string.Empty;

			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.NullValueHandling = NullValueHandling.Ignore;

				serializer.Serialize(sw, this);
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return string.Format("ID: {0} | ATokenExp: {1} | RTokenExp: {2}", Id, AccessExpiry, RefreshExpiry);
		}
	}
}
