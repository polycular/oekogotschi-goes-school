using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Converters;
using JSONApi.JSON;

namespace JSONApi.Converters
{
    public class IdentifierDataConverter : CustomCreationConverter<IIdentifierData>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                IIdentifierData val = new ResourceIdentifierList();
                serializer.Populate(reader, val);
                return val;
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                IIdentifierData val = new ResourceIdentifier();
                serializer.Populate(reader, val);
                return val;
            }

            throw new Exception("Unexpected token or value when parsing data. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
        }

        public override IIdentifierData Create(Type objectType)
        {
            return null;
        }
    }
}
