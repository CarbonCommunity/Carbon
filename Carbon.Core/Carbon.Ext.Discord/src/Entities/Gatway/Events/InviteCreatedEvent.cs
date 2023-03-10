/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Invites;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
	// Token: 0x020000DD RID: 221
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class InviteCreatedEvent
	{
		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x000154A6 File Offset: 0x000136A6
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x000154AE File Offset: 0x000136AE
		[JsonProperty("channel_id")]
		public Snowflake ChannelId { get; set; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x000154B7 File Offset: 0x000136B7
		// (set) Token: 0x060007D6 RID: 2006 RVA: 0x000154BF File Offset: 0x000136BF
		[JsonProperty("code")]
		public string Code { get; set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x000154C8 File Offset: 0x000136C8
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x000154D0 File Offset: 0x000136D0
		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x000154D9 File Offset: 0x000136D9
		// (set) Token: 0x060007DA RID: 2010 RVA: 0x000154E1 File Offset: 0x000136E1
		[JsonProperty("guild_id")]
		public Snowflake? GuildId { get; set; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x000154EA File Offset: 0x000136EA
		// (set) Token: 0x060007DC RID: 2012 RVA: 0x000154F2 File Offset: 0x000136F2
		[JsonProperty("inviter")]
		public DiscordUser Inviter { get; set; }

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x000154FB File Offset: 0x000136FB
		// (set) Token: 0x060007DE RID: 2014 RVA: 0x00015503 File Offset: 0x00013703
		[JsonProperty("max_age")]
		public int MaxAge { get; set; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x0001550C File Offset: 0x0001370C
		// (set) Token: 0x060007E0 RID: 2016 RVA: 0x00015514 File Offset: 0x00013714
		[JsonProperty("max_uses")]
		public int MaxUses { get; set; }

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0001551D File Offset: 0x0001371D
		// (set) Token: 0x060007E2 RID: 2018 RVA: 0x00015525 File Offset: 0x00013725
		[JsonProperty("target_user")]
		public DiscordUser TargetUser { get; set; }

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0001552E File Offset: 0x0001372E
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x00015536 File Offset: 0x00013736
		[JsonProperty("target_user")]
		public TargetUserType TargetUserType { get; set; }

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0001553F File Offset: 0x0001373F
		// (set) Token: 0x060007E6 RID: 2022 RVA: 0x00015547 File Offset: 0x00013747
		[JsonProperty("temporary")]
		public bool? Temporary { get; set; }

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00015550 File Offset: 0x00013750
		// (set) Token: 0x060007E8 RID: 2024 RVA: 0x00015558 File Offset: 0x00013758
		[JsonProperty("uses")]
		public int? Uses { get; set; }
	}
}
