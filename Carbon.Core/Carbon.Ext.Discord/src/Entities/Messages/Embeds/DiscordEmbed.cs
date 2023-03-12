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

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
	// Token: 0x0200006B RID: 107
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class DiscordEmbed
	{
		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060003BD RID: 957 RVA: 0x00010B74 File Offset: 0x0000ED74
		// (set) Token: 0x060003BE RID: 958 RVA: 0x00010B7C File Offset: 0x0000ED7C
		[JsonProperty("title")]
		public string Title { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060003BF RID: 959 RVA: 0x00010B85 File Offset: 0x0000ED85
		// (set) Token: 0x060003C0 RID: 960 RVA: 0x00010B8D File Offset: 0x0000ED8D
		[Obsolete("Embed types should be considered deprecated and might be removed in a future API version")]
		[JsonProperty("type")]
		public string Type { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x00010B96 File Offset: 0x0000ED96
		// (set) Token: 0x060003C2 RID: 962 RVA: 0x00010B9E File Offset: 0x0000ED9E
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x00010BA7 File Offset: 0x0000EDA7
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x00010BAF File Offset: 0x0000EDAF
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00010BB8 File Offset: 0x0000EDB8
		// (set) Token: 0x060003C6 RID: 966 RVA: 0x00010BC0 File Offset: 0x0000EDC0
		[JsonProperty("timestamp")]
		public DateTime? Timestamp { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x00010BC9 File Offset: 0x0000EDC9
		// (set) Token: 0x060003C8 RID: 968 RVA: 0x00010BD1 File Offset: 0x0000EDD1
		[JsonProperty("color")]
		public DiscordColor Color { get; set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00010BDA File Offset: 0x0000EDDA
		// (set) Token: 0x060003CA RID: 970 RVA: 0x00010BE2 File Offset: 0x0000EDE2
		[JsonProperty("footer")]
		public EmbedFooter Footer { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060003CB RID: 971 RVA: 0x00010BEB File Offset: 0x0000EDEB
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00010BF3 File Offset: 0x0000EDF3
		[JsonProperty("image")]
		public EmbedImage Image { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060003CD RID: 973 RVA: 0x00010BFC File Offset: 0x0000EDFC
		// (set) Token: 0x060003CE RID: 974 RVA: 0x00010C04 File Offset: 0x0000EE04
		[JsonProperty("thumbnail")]
		public EmbedThumbnail Thumbnail { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060003CF RID: 975 RVA: 0x00010C0D File Offset: 0x0000EE0D
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x00010C15 File Offset: 0x0000EE15
		[JsonProperty("video")]
		public EmbedVideo Video { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00010C1E File Offset: 0x0000EE1E
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x00010C26 File Offset: 0x0000EE26
		[JsonProperty("provider")]
		public EmbedProvider Provider { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x00010C2F File Offset: 0x0000EE2F
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x00010C37 File Offset: 0x0000EE37
		[JsonProperty("author")]
		public EmbedAuthor Author { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x00010C40 File Offset: 0x0000EE40
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x00010C48 File Offset: 0x0000EE48
		[JsonProperty("fields")]
		public List<EmbedField> Fields { get; set; }
	}
}
