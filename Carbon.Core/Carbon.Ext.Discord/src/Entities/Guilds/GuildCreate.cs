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
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A6 RID: 166
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildCreate
	{
		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000651 RID: 1617 RVA: 0x000141A1 File Offset: 0x000123A1
		// (set) Token: 0x06000652 RID: 1618 RVA: 0x000141A9 File Offset: 0x000123A9
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x000141B2 File Offset: 0x000123B2
		// (set) Token: 0x06000654 RID: 1620 RVA: 0x000141BA File Offset: 0x000123BA
		[Obsolete("Deprecated in Discord API")]
		[JsonProperty("region")]
		public string Region { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000655 RID: 1621 RVA: 0x000141C3 File Offset: 0x000123C3
		// (set) Token: 0x06000656 RID: 1622 RVA: 0x000141CB File Offset: 0x000123CB
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x000141D4 File Offset: 0x000123D4
		// (set) Token: 0x06000658 RID: 1624 RVA: 0x000141DC File Offset: 0x000123DC
		[JsonProperty("verification_level")]
		public GuildVerificationLevel? VerificationLevel { get; set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x000141E5 File Offset: 0x000123E5
		// (set) Token: 0x0600065A RID: 1626 RVA: 0x000141ED File Offset: 0x000123ED
		[JsonProperty("default_message_notifications")]
		public DefaultNotificationLevel? DefaultMessageNotifications { get; set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x000141F6 File Offset: 0x000123F6
		// (set) Token: 0x0600065C RID: 1628 RVA: 0x000141FE File Offset: 0x000123FE
		[JsonProperty("explicit_content_filter")]
		public ExplicitContentFilterLevel? ExplicitContentFilter { get; set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x00014207 File Offset: 0x00012407
		// (set) Token: 0x0600065E RID: 1630 RVA: 0x0001420F File Offset: 0x0001240F
		[JsonProperty("roles")]
		public List<DiscordRole> Roles { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x00014218 File Offset: 0x00012418
		// (set) Token: 0x06000660 RID: 1632 RVA: 0x00014220 File Offset: 0x00012420
		[JsonProperty("channels")]
		public List<DiscordChannel> Channels { get; set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x00014229 File Offset: 0x00012429
		// (set) Token: 0x06000662 RID: 1634 RVA: 0x00014231 File Offset: 0x00012431
		[JsonProperty("afk_channel_id")]
		public Snowflake AfkChannelId { get; set; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x0001423A File Offset: 0x0001243A
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00014242 File Offset: 0x00012442
		[JsonProperty("afk_timeout")]
		public int? AfkTimeout { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x0001424B File Offset: 0x0001244B
		// (set) Token: 0x06000666 RID: 1638 RVA: 0x00014253 File Offset: 0x00012453
		[JsonProperty("system_channel_id")]
		public Snowflake SystemChannelId { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x0001425C File Offset: 0x0001245C
		// (set) Token: 0x06000668 RID: 1640 RVA: 0x00014264 File Offset: 0x00012464
		[JsonProperty("system_channel_flags")]
		public SystemChannelFlags? SystemChannelFlags { get; set; }
	}
}
