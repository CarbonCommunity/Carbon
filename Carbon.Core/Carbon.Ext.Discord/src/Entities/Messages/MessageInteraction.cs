/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Interactions;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000067 RID: 103
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageInteraction
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x00010AB9 File Offset: 0x0000ECB9
		// (set) Token: 0x060003A5 RID: 933 RVA: 0x00010AC1 File Offset: 0x0000ECC1
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00010ACA File Offset: 0x0000ECCA
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00010AD2 File Offset: 0x0000ECD2
		[JsonProperty("type")]
		public InteractionType Type { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00010ADB File Offset: 0x0000ECDB
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x00010AE3 File Offset: 0x0000ECE3
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00010AEC File Offset: 0x0000ECEC
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00010AF4 File Offset: 0x0000ECF4
		[JsonProperty("user")]
		public DiscordUser User { get; set; }
	}
}
