/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Users.Connections
{
	// Token: 0x02000050 RID: 80
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class Connection
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600027E RID: 638 RVA: 0x0000F2BC File Offset: 0x0000D4BC
		// (set) Token: 0x0600027F RID: 639 RVA: 0x0000F2C4 File Offset: 0x0000D4C4
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000F2CD File Offset: 0x0000D4CD
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0000F2D5 File Offset: 0x0000D4D5
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0000F2DE File Offset: 0x0000D4DE
		// (set) Token: 0x06000283 RID: 643 RVA: 0x0000F2E6 File Offset: 0x0000D4E6
		[JsonProperty("type")]
		public ConnectionType Type { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000F2EF File Offset: 0x0000D4EF
		// (set) Token: 0x06000285 RID: 645 RVA: 0x0000F2F7 File Offset: 0x0000D4F7
		[JsonProperty("revoked")]
		public bool? Revoked { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000F300 File Offset: 0x0000D500
		// (set) Token: 0x06000287 RID: 647 RVA: 0x0000F308 File Offset: 0x0000D508
		[JsonConverter(typeof(HashListConverter<Integration>))]
		[JsonProperty("integrations")]
		public Hash<Snowflake, Integration> Integrations { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000F311 File Offset: 0x0000D511
		// (set) Token: 0x06000289 RID: 649 RVA: 0x0000F319 File Offset: 0x0000D519
		[JsonProperty("verified")]
		public bool Verified { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000F322 File Offset: 0x0000D522
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000F32A File Offset: 0x0000D52A
		[JsonProperty("friend_sync")]
		public bool FriendSync { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000F333 File Offset: 0x0000D533
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000F33B File Offset: 0x0000D53B
		[JsonProperty("show_activity")]
		public bool ShowActivity { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000F344 File Offset: 0x0000D544
		// (set) Token: 0x0600028F RID: 655 RVA: 0x0000F34C File Offset: 0x0000D54C
		[JsonProperty("visibility")]
		public ConnectionVisibilityType Visibility { get; set; }
	}
}
