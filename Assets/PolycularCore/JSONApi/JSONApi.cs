using JSONApi.ContractResolvers;
using JSONApi.JSON;
using Newtonsoft.Json;


namespace Assets.JSONApi
{
	public static class JSONApi
	{
		public static Object Decode(string str)
		{
			return JsonConvert.DeserializeObject<Object>(str);
		}

		public static string Encode(Object jsonObject)
		{
			return JsonConvert.SerializeObject(
				jsonObject,
				Formatting.None,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					ContractResolver = new ToCamelCaseContractResolver()
				});
		}
	}
}
