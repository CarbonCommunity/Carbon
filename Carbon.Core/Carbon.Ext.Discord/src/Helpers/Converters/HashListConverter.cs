using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers.Converters
{
    /// <summary>
    /// Converts to and from a list in JSON to a hash
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class HashListConverter<TValue> : JsonConverter where TValue : ISnowflakeEntity
    {
        /// <summary>
        /// Read an array in JSON as a hash
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);

            Hash<Snowflake, TValue> data = new Hash<Snowflake, TValue>();
            foreach (JToken token in array)
            {
                TValue value = token.ToObject<TValue>();
                data[value.Id] = value;
            }

            return data;
        }
        
        /// <summary>
        /// Write a hash as a list in JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Hash<Snowflake, TValue> data = (Hash<Snowflake, TValue>) value;
            
            writer.WriteStartArray();
            foreach (TValue tValue in data.Values)
            {
                serializer.Serialize(writer, tValue);
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// Can we convert the given type
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<TValue>) || objectType == typeof(Hash<Snowflake, TValue>);
        }
    }
}