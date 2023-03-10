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
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Interactions.MessageComponents
{
	// Token: 0x02000084 RID: 132
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ActionRowComponent : BaseComponent
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x00011C50 File Offset: 0x0000FE50
		[JsonConverter(typeof(MessageComponentsConverter))]
		[JsonProperty("components")]
		public List<BaseComponent> Components { get; } = new List<BaseComponent>();

		// Token: 0x060004CE RID: 1230 RVA: 0x00011C58 File Offset: 0x0000FE58
		public ActionRowComponent()
		{
			base.Type = MessageComponentType.ActionRow;
		}
	}
}
