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
	// Token: 0x020000C0 RID: 192
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class ScheduledEventUpdate
	{
		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x00014E6E File Offset: 0x0001306E
		// (set) Token: 0x0600072D RID: 1837 RVA: 0x00014E76 File Offset: 0x00013076
		[JsonProperty("channel_id")]
		public Snowflake? ChannelId { get; set; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600072E RID: 1838 RVA: 0x00014E7F File Offset: 0x0001307F
		// (set) Token: 0x0600072F RID: 1839 RVA: 0x00014E87 File Offset: 0x00013087
		[JsonProperty("entity_metadata")]
		public ScheduledEventEntityMetadata EntityMetadata { get; set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x00014E90 File Offset: 0x00013090
		// (set) Token: 0x06000731 RID: 1841 RVA: 0x00014E98 File Offset: 0x00013098
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x00014EA1 File Offset: 0x000130A1
		// (set) Token: 0x06000733 RID: 1843 RVA: 0x00014EA9 File Offset: 0x000130A9
		[JsonProperty("privacy_level")]
		public ScheduledEventPrivacyLevel? PrivacyLevel { get; set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x00014EB2 File Offset: 0x000130B2
		// (set) Token: 0x06000735 RID: 1845 RVA: 0x00014EBA File Offset: 0x000130BA
		[JsonProperty("scheduled_start_time")]
		public DateTime? ScheduledStartTime { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000736 RID: 1846 RVA: 0x00014EC3 File Offset: 0x000130C3
		// (set) Token: 0x06000737 RID: 1847 RVA: 0x00014ECB File Offset: 0x000130CB
		[JsonProperty("scheduled_end_time ")]
		public DateTime? ScheduledEndTime { get; set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000738 RID: 1848 RVA: 0x00014ED4 File Offset: 0x000130D4
		// (set) Token: 0x06000739 RID: 1849 RVA: 0x00014EDC File Offset: 0x000130DC
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x00014EE5 File Offset: 0x000130E5
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x00014EED File Offset: 0x000130ED
		[JsonProperty("entity_type")]
		public ScheduledEventEntityType? EntityType { get; set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x00014EF6 File Offset: 0x000130F6
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x00014EFE File Offset: 0x000130FE
		[JsonProperty("status")]
		public ScheduledEventStatus? Status { get; set; }
	}
}
