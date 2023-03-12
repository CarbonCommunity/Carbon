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
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Stickers;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000AE RID: 174
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class GuildPreview
	{
		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001458C File Offset: 0x0001278C
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x00014594 File Offset: 0x00012794
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060006A0 RID: 1696 RVA: 0x0001459D File Offset: 0x0001279D
		// (set) Token: 0x060006A1 RID: 1697 RVA: 0x000145A5 File Offset: 0x000127A5
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x000145AE File Offset: 0x000127AE
		// (set) Token: 0x060006A3 RID: 1699 RVA: 0x000145B6 File Offset: 0x000127B6
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x000145BF File Offset: 0x000127BF
		// (set) Token: 0x060006A5 RID: 1701 RVA: 0x000145C7 File Offset: 0x000127C7
		[JsonProperty("splash")]
		public string Splash { get; set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x000145D0 File Offset: 0x000127D0
		// (set) Token: 0x060006A7 RID: 1703 RVA: 0x000145D8 File Offset: 0x000127D8
		[JsonProperty("discovery_splash")]
		public string DiscoverySplash { get; set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x000145E1 File Offset: 0x000127E1
		// (set) Token: 0x060006A9 RID: 1705 RVA: 0x000145E9 File Offset: 0x000127E9
		[JsonProperty("emojis")]
		public List<DiscordEmoji> Emojis { get; set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x000145F2 File Offset: 0x000127F2
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x000145FA File Offset: 0x000127FA
		[JsonProperty("features")]
		public List<GuildFeatures> Features { get; set; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x00014603 File Offset: 0x00012803
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x0001460B File Offset: 0x0001280B
		[JsonProperty("approximate_member_count")]
		public int? ApproximateMemberCount { get; set; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x00014614 File Offset: 0x00012814
		// (set) Token: 0x060006AF RID: 1711 RVA: 0x0001461C File Offset: 0x0001281C
		[JsonProperty("approximate_presence_count")]
		public int? ApproximatePresenceCount { get; set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x00014625 File Offset: 0x00012825
		// (set) Token: 0x060006B1 RID: 1713 RVA: 0x0001462D File Offset: 0x0001282D
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x00014636 File Offset: 0x00012836
		// (set) Token: 0x060006B3 RID: 1715 RVA: 0x0001463E File Offset: 0x0001283E
		[JsonProperty("stickers")]
		public List<DiscordSticker> Stickers { get; set; }
	}
}
