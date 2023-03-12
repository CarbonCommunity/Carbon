/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Utilities;

namespace Oxide.Ext.Discord.Helpers.Converters
{
	// Token: 0x0200002B RID: 43
	public class DiscordEnumConverter : JsonConverter
	{
		// Token: 0x0600017B RID: 379 RVA: 0x0000C8AC File Offset: 0x0000AAAC
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			bool flag = value == null;
			if (flag)
			{
				writer.WriteNull();
			}
			else
			{
				Enum @enum = (Enum)value;
				string text = @enum.ToString("G");
				bool flag2 = char.IsNumber(text[0]) || text[0] == '-';
				if (flag2)
				{
					writer.WriteValue(value);
				}
				else
				{
					string text2 = JsonEnumUtils.ToEnumName(@enum.GetType(), text);
					bool flag3 = !string.IsNullOrEmpty(text2);
					if (flag3)
					{
						writer.WriteValue(text2);
					}
				}
			}
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000C934 File Offset: 0x0000AB34
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = this.IsNullable(objectType);
			bool flag2 = reader.TokenType == (JsonToken)11;
			object result;
			if (flag2)
			{
				bool flag3 = !flag;
				if (flag3)
				{
					throw new JsonException(string.Format("Cannot convert null value to {0}.", objectType));
				}
				result = null;
			}
			else
			{
				bool flag4 = reader.TokenType == (JsonToken)7;
				if (flag4)
				{
					bool flag5 = Enum.IsDefined(objectType, reader.Value.ToString());
					if (flag5)
					{
						result = Enum.Parse(objectType, reader.Value.ToString());
					}
					else
					{
						result = this.GetDefault(objectType);
					}
				}
				else
				{
					bool flag6 = reader.TokenType == (JsonToken)9;
					if (!flag6)
					{
						throw new JsonException(string.Format("Unexpected token {0} when parsing enum.", reader.TokenType));
					}
					string text = reader.Value.ToString();
					string value = JsonEnumUtils.FromEnumName(objectType, text) ?? text;
					bool flag7 = Enum.IsDefined(objectType, value);
					if (flag7)
					{
						result = Enum.Parse(objectType, value);
					}
					else
					{
						result = this.GetDefault(objectType);
					}
				}
			}
			return result;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000CA30 File Offset: 0x0000AC30
		public override bool CanConvert(Type objectType)
		{
			bool result;
			if (objectType != null)
			{
				Type type = this.IsNullable(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
				result = (type != null && type.IsEnum);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000CA6C File Offset: 0x0000AC6C
		private object GetDefault(Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000CA90 File Offset: 0x0000AC90
		private bool IsNullable(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}
