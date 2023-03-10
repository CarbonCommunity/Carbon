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
using Oxide.Ext.Discord.Entities.Api;

namespace Oxide.Ext.Discord.Entities.Voice
{
	// Token: 0x02000049 RID: 73
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class VoiceRegion
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000E754 File Offset: 0x0000C954
		// (set) Token: 0x06000214 RID: 532 RVA: 0x0000E75C File Offset: 0x0000C95C
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000E765 File Offset: 0x0000C965
		// (set) Token: 0x06000216 RID: 534 RVA: 0x0000E76D File Offset: 0x0000C96D
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000E776 File Offset: 0x0000C976
		// (set) Token: 0x06000218 RID: 536 RVA: 0x0000E77E File Offset: 0x0000C97E
		[Obsolete("This field is no longer sent by discord")]
		[JsonProperty("vip")]
		public bool Vip { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000219 RID: 537 RVA: 0x0000E787 File Offset: 0x0000C987
		// (set) Token: 0x0600021A RID: 538 RVA: 0x0000E78F File Offset: 0x0000C98F
		[JsonProperty("optimal")]
		public bool Optimal { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000E798 File Offset: 0x0000C998
		// (set) Token: 0x0600021C RID: 540 RVA: 0x0000E7A0 File Offset: 0x0000C9A0
		[JsonProperty("deprecated")]
		public bool Deprecated { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0000E7A9 File Offset: 0x0000C9A9
		// (set) Token: 0x0600021E RID: 542 RVA: 0x0000E7B1 File Offset: 0x0000C9B1
		[JsonProperty("custom")]
		public bool Custom { get; set; }

		// Token: 0x0600021F RID: 543 RVA: 0x0000E7BA File Offset: 0x0000C9BA
		public static void ListVoiceRegions(DiscordClient client, Action<List<VoiceRegion>> callback = null, Action<RestError> error = null)
		{
			client.Bot.Rest.DoRequest<List<VoiceRegion>>("/voice/regions", RequestMethod.GET, null, callback, error);
		}
	}
}
