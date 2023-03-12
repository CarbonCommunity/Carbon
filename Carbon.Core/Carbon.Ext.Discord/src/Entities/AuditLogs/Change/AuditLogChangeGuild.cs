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
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000112 RID: 274
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeGuild
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x060009F5 RID: 2549 RVA: 0x000175FA File Offset: 0x000157FA
		// (set) Token: 0x060009F6 RID: 2550 RVA: 0x00017602 File Offset: 0x00015802
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x060009F7 RID: 2551 RVA: 0x0001760B File Offset: 0x0001580B
		// (set) Token: 0x060009F8 RID: 2552 RVA: 0x00017613 File Offset: 0x00015813
		[JsonProperty("icon_hash")]
		public string IconHash { get; set; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x060009F9 RID: 2553 RVA: 0x0001761C File Offset: 0x0001581C
		// (set) Token: 0x060009FA RID: 2554 RVA: 0x00017624 File Offset: 0x00015824
		[JsonProperty("splash_hash")]
		public string SplashHash { get; set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x060009FB RID: 2555 RVA: 0x0001762D File Offset: 0x0001582D
		// (set) Token: 0x060009FC RID: 2556 RVA: 0x00017635 File Offset: 0x00015835
		[JsonProperty("owner_id")]
		public Snowflake OwnerId { get; set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x060009FD RID: 2557 RVA: 0x0001763E File Offset: 0x0001583E
		// (set) Token: 0x060009FE RID: 2558 RVA: 0x00017646 File Offset: 0x00015846
		[JsonProperty("region")]
		public string Region { get; set; }

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x060009FF RID: 2559 RVA: 0x0001764F File Offset: 0x0001584F
		// (set) Token: 0x06000A00 RID: 2560 RVA: 0x00017657 File Offset: 0x00015857
		[JsonProperty("afk_channel_id")]
		public Snowflake AfkChannelId { get; set; }

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000A01 RID: 2561 RVA: 0x00017660 File Offset: 0x00015860
		// (set) Token: 0x06000A02 RID: 2562 RVA: 0x00017668 File Offset: 0x00015868
		[JsonProperty("afk_timeout")]
		public int? AfkTimeout { get; set; }

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000A03 RID: 2563 RVA: 0x00017671 File Offset: 0x00015871
		// (set) Token: 0x06000A04 RID: 2564 RVA: 0x00017679 File Offset: 0x00015879
		[JsonProperty("mfa_level")]
		public int? MfaLevel { get; set; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000A05 RID: 2565 RVA: 0x00017682 File Offset: 0x00015882
		// (set) Token: 0x06000A06 RID: 2566 RVA: 0x0001768A File Offset: 0x0001588A
		[JsonProperty("verification_level")]
		public int? VerificationLevel { get; set; }

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000A07 RID: 2567 RVA: 0x00017693 File Offset: 0x00015893
		// (set) Token: 0x06000A08 RID: 2568 RVA: 0x0001769B File Offset: 0x0001589B
		[JsonProperty("explicit_content_filter")]
		public int? ExplicitContentFilter { get; set; }

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000A09 RID: 2569 RVA: 0x000176A4 File Offset: 0x000158A4
		// (set) Token: 0x06000A0A RID: 2570 RVA: 0x000176AC File Offset: 0x000158AC
		[JsonProperty("default_message_notifications")]
		public int? DefaultMessageNotifications { get; set; }

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000A0B RID: 2571 RVA: 0x000176B5 File Offset: 0x000158B5
		// (set) Token: 0x06000A0C RID: 2572 RVA: 0x000176BD File Offset: 0x000158BD
		[JsonProperty("vanity_url_code")]
		public string VanityUrlCode { get; set; }

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000A0D RID: 2573 RVA: 0x000176C6 File Offset: 0x000158C6
		// (set) Token: 0x06000A0E RID: 2574 RVA: 0x000176CE File Offset: 0x000158CE
		[JsonProperty("$add")]
		public List<DiscordRole> Add { get; set; }

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x000176D7 File Offset: 0x000158D7
		// (set) Token: 0x06000A10 RID: 2576 RVA: 0x000176DF File Offset: 0x000158DF
		[JsonProperty("$remove")]
		public List<DiscordRole> Remove { get; set; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000A11 RID: 2577 RVA: 0x000176E8 File Offset: 0x000158E8
		// (set) Token: 0x06000A12 RID: 2578 RVA: 0x000176F0 File Offset: 0x000158F0
		[JsonProperty("prune_delete_days")]
		public int? PruneDeleteDays { get; set; }

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000A13 RID: 2579 RVA: 0x000176F9 File Offset: 0x000158F9
		// (set) Token: 0x06000A14 RID: 2580 RVA: 0x00017701 File Offset: 0x00015901
		[JsonProperty("widget_enabled")]
		public bool WidgetEnabled { get; set; }

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000A15 RID: 2581 RVA: 0x0001770A File Offset: 0x0001590A
		// (set) Token: 0x06000A16 RID: 2582 RVA: 0x00017712 File Offset: 0x00015912
		[JsonProperty("widget_channel_id")]
		public Snowflake WidgetChannelId { get; set; }

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x0001771B File Offset: 0x0001591B
		// (set) Token: 0x06000A18 RID: 2584 RVA: 0x00017723 File Offset: 0x00015923
		[JsonProperty("system_channel_id")]
		public Snowflake SystemChannelId { get; set; }
	}
}
