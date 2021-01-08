using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BLL.Common
{
    public class JsonInt32Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JToken jt = JValue.ReadFrom(reader);
                return string.IsNullOrEmpty(jt.ToString()) ? (int?)null : jt.Value<int>();
            }
            catch (OverflowException oEx)
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return (typeof(int).Equals(objectType) || typeof(int?).Equals(objectType));
        }
    }
}