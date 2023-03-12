/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Helpers.Converters
{
	// Token: 0x0200002C RID: 44
	public class HashListConverter<TValue> : JsonConverter where TValue : ISnowflakeEntity
	{
		// Token: 0x06000181 RID: 385 RVA: 0x0000CACC File Offset: 0x0000ACCC
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JArray jarray = JArray.Load(reader);
			Hash<Snowflake, TValue> hash = new Hash<Snowflake, TValue>();
			foreach (JToken jtoken in jarray)
			{
				TValue tvalue = jtoken.ToObject<TValue>();
				hash[tvalue.Id] = tvalue;
			}
			return hash;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000CB44 File Offset: 0x0000AD44
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Hash<Snowflake, TValue> hash = (Hash<Snowflake, TValue>)value;
			writer.WriteStartArray();
			foreach (TValue tvalue in hash.Values)
			{
				serializer.Serialize(writer, tvalue);
			}
			writer.WriteEndArray();
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000CBB4 File Offset: 0x0000ADB4
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(List<TValue>) || objectType == typeof(Hash<Snowflake, TValue>);
		}
	}
}
