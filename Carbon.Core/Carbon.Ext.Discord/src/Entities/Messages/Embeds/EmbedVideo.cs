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
	// Token: 0x02000072 RID: 114
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedVideo
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x00010E84 File Offset: 0x0000F084
		// (set) Token: 0x0600040D RID: 1037 RVA: 0x00010E8C File Offset: 0x0000F08C
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00010E95 File Offset: 0x0000F095
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00010E9D File Offset: 0x0000F09D
		[JsonProperty("proxy_url")]
		public string ProxyUrl { get; set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x00010EA6 File Offset: 0x0000F0A6
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00010EAE File Offset: 0x0000F0AE
		[JsonProperty("height")]
		public int? Height { get; set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00010EB7 File Offset: 0x0000F0B7
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x00010EBF File Offset: 0x0000F0BF
		[JsonProperty("width")]
		public int? Width { get; set; }

		// Token: 0x06000414 RID: 1044 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedVideo()
		{
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00010EC8 File Offset: 0x0000F0C8
		public EmbedVideo(string url, int? height, int? width, string proxyUrl)
		{
			this.Url = url;
			this.ProxyUrl = proxyUrl;
			this.Height = height;
			this.Width = width;
		}
	}
}
