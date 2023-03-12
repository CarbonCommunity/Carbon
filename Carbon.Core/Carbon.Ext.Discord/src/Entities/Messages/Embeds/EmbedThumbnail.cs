/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
	// Token: 0x02000071 RID: 113
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedThumbnail
	{
		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x00010E15 File Offset: 0x0000F015
		// (set) Token: 0x06000403 RID: 1027 RVA: 0x00010E1D File Offset: 0x0000F01D
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x00010E26 File Offset: 0x0000F026
		// (set) Token: 0x06000405 RID: 1029 RVA: 0x00010E2E File Offset: 0x0000F02E
		[JsonProperty("proxy_url")]
		public string ProxyUrl { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x00010E37 File Offset: 0x0000F037
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x00010E3F File Offset: 0x0000F03F
		[JsonProperty("height")]
		public int? Height { get; set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x00010E48 File Offset: 0x0000F048
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x00010E50 File Offset: 0x0000F050
		[JsonProperty("width")]
		public int? Width { get; set; }

		// Token: 0x0600040A RID: 1034 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedThumbnail()
		{
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00010E59 File Offset: 0x0000F059
		public EmbedThumbnail(string url, int? height = null, int? width = null, string proxyUrl = null)
		{
			this.Url = url;
			this.ProxyUrl = proxyUrl;
			this.Height = height;
			this.Width = width;
		}
	}
}
