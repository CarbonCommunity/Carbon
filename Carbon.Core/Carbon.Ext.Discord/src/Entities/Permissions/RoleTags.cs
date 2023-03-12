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
	// Token: 0x0200005F RID: 95
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class RoleTags
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000FE4D File Offset: 0x0000E04D
		// (set) Token: 0x06000307 RID: 775 RVA: 0x0000FE55 File Offset: 0x0000E055
		[JsonProperty("bot_id")]
		public Snowflake? BotId { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000FE5E File Offset: 0x0000E05E
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000FE66 File Offset: 0x0000E066
		[JsonProperty("integration_id")]
		public Snowflake? IntegrationId { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0000FE6F File Offset: 0x0000E06F
		// (set) Token: 0x0600030B RID: 779 RVA: 0x0000FE77 File Offset: 0x0000E077
		[JsonProperty("premium_subscriber")]
		public bool? PremiumSubscriber { get; set; }
	}
}
