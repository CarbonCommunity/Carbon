using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Utilities;

namespace Oxide.Ext.Discord.Helpers.Converters
{
    /// <summary>
    /// Handles deserializing JSON values as strings. If they value doesn't exist return the default value.
    /// </summary>
    public class DiscordEnumConverter : JsonConverter
    {
        /// <summary>
        /// Write Enum value to Discord Enum String
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            Enum enumValue = (Enum) value;
            string enumText = enumValue.ToString("G");
            if (char.IsNumber(enumText[0]) || enumText[0] == '-')
            {
                writer.WriteValue(value);
                return;
            }

            string enumName = JsonEnumUtils.ToEnumName(enumValue.GetType(), enumText);
            if (!string.IsNullOrEmpty(enumName))
            {
                writer.WriteValue(enumName);
            }
        }

        /// <summary>
        /// Read enum value from Discord Enum String
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullable(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!isNullable)
                {
                    throw new JsonException($"Cannot convert null value to {objectType}.");
                }

                return null;
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                if (Enum.IsDefined(objectType, reader.Value.ToString()))
                {
                    return Enum.Parse(objectType, reader.Value.ToString());
                }

                return GetDefault(objectType);
            }

            if (reader.TokenType == JsonToken.String)
            {
                string enumValue = reader.Value.ToString();
                string enumName = JsonEnumUtils.FromEnumName(objectType, enumValue) ?? enumValue;
                if (Enum.IsDefined(objectType, enumName))
                {
                    return Enum.Parse(objectType, enumName);
                }
                
                return GetDefault(objectType);
            }

            throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum.");
        }
        
        /// <summary>
        /// Checks if this type is enum or nullable enum
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && ((IsNullable(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType)?.IsEnum ?? false);
        }

        private object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        
        private bool IsNullable(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}