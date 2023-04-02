using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;

namespace Oxide.Ext.Discord.Helpers.Converters
{
    /// <summary>
    /// Converter for list of message components
    /// </summary>
    public class MessageComponentsConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        /// <exception cref="NotSupportedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Populate the correct types in components instead of just the BaseComponent
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            if (!(existingValue is List<BaseComponent> components))
            {
                components = new List<BaseComponent>();
            }

            foreach (JToken token in array)
            {
                MessageComponentType type = (MessageComponentType)Enum.Parse(typeof(MessageComponentType), token["type"].ToString());
                switch (type)
                {
                    case MessageComponentType.Button:
                        components.Add(token.ToObject<ButtonComponent>());
                        break;
                        
                    case MessageComponentType.SelectMenu:
                        components.Add(token.ToObject<SelectMenuComponent>());
                        break;
                }
            }

            return components;
        }

        /// <summary>
        /// Returns if this can convert the value
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<BaseComponent>);
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite => false;
    }
}