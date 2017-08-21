using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JSONApi.JSON
{
	public class Resource : IData
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public Dictionary<string, JToken> Attributes { get; set; }

        public Dictionary<string, Relationship> Relationships { get; set; }

        public Links Links { get; set; }

        public Meta Meta { get; set; }
    }
}