/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Messages
{
	// Token: 0x02000063 RID: 99
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class MessageAttachment : ISnowflakeEntity
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00010733 File Offset: 0x0000E933
		// (set) Token: 0x0600036D RID: 877 RVA: 0x0001073B File Offset: 0x0000E93B
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00010744 File Offset: 0x0000E944
		// (set) Token: 0x0600036F RID: 879 RVA: 0x0001074C File Offset: 0x0000E94C
		[JsonProperty("filename")]
		public string Filename { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00010755 File Offset: 0x0000E955
		// (set) Token: 0x06000371 RID: 881 RVA: 0x0001075D File Offset: 0x0000E95D
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00010766 File Offset: 0x0000E966
		// (set) Token: 0x06000373 RID: 883 RVA: 0x0001076E File Offset: 0x0000E96E
		[JsonProperty("content_type")]
		public string ContentType { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00010777 File Offset: 0x0000E977
		// (set) Token: 0x06000375 RID: 885 RVA: 0x0001077F File Offset: 0x0000E97F
		[JsonProperty("size")]
		public int? Size { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00010788 File Offset: 0x0000E988
		// (set) Token: 0x06000377 RID: 887 RVA: 0x00010790 File Offset: 0x0000E990
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00010799 File Offset: 0x0000E999
		// (set) Token: 0x06000379 RID: 889 RVA: 0x000107A1 File Offset: 0x0000E9A1
		[JsonProperty("proxy_url")]
		public string ProxyUrl { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600037A RID: 890 RVA: 0x000107AA File Offset: 0x0000E9AA
		// (set) Token: 0x0600037B RID: 891 RVA: 0x000107B2 File Offset: 0x0000E9B2
		[JsonProperty("height")]
		public int? Height { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600037C RID: 892 RVA: 0x000107BB File Offset: 0x0000E9BB
		// (set) Token: 0x0600037D RID: 893 RVA: 0x000107C3 File Offset: 0x0000E9C3
		[JsonProperty("width")]
		public int? Width { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600037E RID: 894 RVA: 0x000107CC File Offset: 0x0000E9CC
		// (set) Token: 0x0600037F RID: 895 RVA: 0x000107D4 File Offset: 0x0000E9D4
		[JsonProperty("ephemeral")]
		public bool? Ephemeral { get; set; }
	}
}
