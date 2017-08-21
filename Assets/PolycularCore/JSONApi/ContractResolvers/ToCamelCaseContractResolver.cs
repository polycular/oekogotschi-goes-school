using Newtonsoft.Json.Serialization;
using Polycular.Utilities;

namespace JSONApi.ContractResolvers
{
	public class ToCamelCaseContractResolver : DefaultContractResolver
    {
        protected internal override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToCamelCase();
        }
    }
}