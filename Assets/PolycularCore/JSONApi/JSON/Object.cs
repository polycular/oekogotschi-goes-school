using Newtonsoft.Json;
using System.Collections.Generic;
using JSONApi.Converters;

namespace JSONApi.JSON
{
    public class Object
    {
        [JsonConverter(typeof(DataConverter))]
        public IData Data { get; set; }

        public List<Error> Errors { get; set; }

        public Meta Meta { get; set; }

        public Links Links { get; set; }

        public List<Resource> Included { get; set; }
    }
}