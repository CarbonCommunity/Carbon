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
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;

namespace Oxide.Ext.Discord.Helpers.Converters
{
	// Token: 0x0200002D RID: 45
	public class MessageComponentsConverter : JsonConverter
	{
		// Token: 0x06000185 RID: 389 RVA: 0x0000CBEB File Offset: 0x0000ADEB
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000CBF4 File Offset: 0x0000ADF4
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JArray jarray = JArray.Load(reader);
			List<BaseComponent> list = existingValue as List<BaseComponent>;
			bool flag = list == null;
			if (flag)
			{
				list = new List<BaseComponent>();
			}
			foreach (JToken jtoken in jarray)
			{
				MessageComponentType messageComponentType = (MessageComponentType)Enum.Parse(typeof(MessageComponentType), jtoken["type"].ToString());
				MessageComponentType messageComponentType2 = messageComponentType;
				MessageComponentType messageComponentType3 = messageComponentType2;
				if (messageComponentType3 != MessageComponentType.Button)
				{
					if (messageComponentType3 == MessageComponentType.SelectMenu)
					{
						list.Add(jtoken.ToObject<SelectMenuComponent>());
					}
				}
				else
				{
					list.Add(jtoken.ToObject<ButtonComponent>());
				}
			}
			return list;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(List<BaseComponent>);
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000188 RID: 392 RVA: 0x0000CCE2 File Offset: 0x0000AEE2
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}
	}
}
