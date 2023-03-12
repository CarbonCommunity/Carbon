/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Permissions
{
	// Token: 0x0200005C RID: 92
	public class DiscordColorConverter : JsonConverter
	{
		// Token: 0x060002E5 RID: 741 RVA: 0x0000FB38 File Offset: 0x0000DD38
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((DiscordColor)value).Color);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000FB5C File Offset: 0x0000DD5C
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool flag = reader.TokenType == (JsonToken)11;
			object result;
			if (flag)
			{
				bool flag2 = !this.IsNullable(objectType);
				if (flag2)
				{
					throw new JsonException(string.Format("Cannot convert null value to {0}. Path: {1}", objectType, reader.Path));
				}
				result = null;
			}
			else
			{
				bool flag3 = reader.TokenType == (JsonToken)7;
				if (!flag3)
				{
					throw new JsonException(string.Format("Unexpected token {0} when parsing discord color. Path: {1}", reader.TokenType, reader.Path));
				}
				result = new DiscordColor(uint.Parse(reader.Value.ToString()));
			}
			return result;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000FBF0 File Offset: 0x0000DDF0
		public override bool CanConvert(Type objectType)
		{
			return objectType != null && (this.IsNullable(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType) == typeof(DiscordColor);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000FC30 File Offset: 0x0000DE30
		private bool IsNullable(Type objectType)
		{
			return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}
