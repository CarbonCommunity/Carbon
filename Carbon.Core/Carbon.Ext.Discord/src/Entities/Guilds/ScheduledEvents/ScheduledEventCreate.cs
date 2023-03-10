/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
	// Token: 0x020000BA RID: 186
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ScheduledEventCreate
	{
		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x00014D41 File Offset: 0x00012F41
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x00014D49 File Offset: 0x00012F49
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x00014D52 File Offset: 0x00012F52
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x00014D5A File Offset: 0x00012F5A
		[JsonProperty("entity_metadata")]
		public ScheduledEventEntityMetadata EntityMetadata { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x00014D63 File Offset: 0x00012F63
		// (set) Token: 0x06000718 RID: 1816 RVA: 0x00014D6B File Offset: 0x00012F6B
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x00014D74 File Offset: 0x00012F74
		// (set) Token: 0x0600071A RID: 1818 RVA: 0x00014D7C File Offset: 0x00012F7C
		[JsonProperty("privacy_level")]
		public ScheduledEventPrivacyLevel PrivacyLevel { get; set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x00014D85 File Offset: 0x00012F85
		// (set) Token: 0x0600071C RID: 1820 RVA: 0x00014D8D File Offset: 0x00012F8D
		[JsonProperty("scheduled_start_time")]
		public DateTime ScheduledStartTime { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x00014D96 File Offset: 0x00012F96
		// (set) Token: 0x0600071E RID: 1822 RVA: 0x00014D9E File Offset: 0x00012F9E
		[JsonProperty("scheduled_end_time ")]
		public DateTime? ScheduledEndTime { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x00014DA7 File Offset: 0x00012FA7
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x00014DAF File Offset: 0x00012FAF
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x00014DB8 File Offset: 0x00012FB8
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x00014DC0 File Offset: 0x00012FC0
		[JsonProperty("entity_type")]
		public ScheduledEventEntityType EntityType { get; set; }
	}
}
