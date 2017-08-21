using JSONApi.Converters;
using Newtonsoft.Json;

namespace JSONApi.JSON
{
    public class Relationship
    {
        public Links Links { get; set; }

        public Meta Meta { get; set; }

        [JsonConverter(typeof(IdentifierDataConverter))]
        public IIdentifierData Data { get; set; }
    }
}