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
	// Token: 0x0200006F RID: 111
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedImage
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00010D6A File Offset: 0x0000EF6A
		// (set) Token: 0x060003F3 RID: 1011 RVA: 0x00010D72 File Offset: 0x0000EF72
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00010D7B File Offset: 0x0000EF7B
		// (set) Token: 0x060003F5 RID: 1013 RVA: 0x00010D83 File Offset: 0x0000EF83
		[JsonProperty("proxy_url")]
		public string ProxyUrl { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00010D8C File Offset: 0x0000EF8C
		// (set) Token: 0x060003F7 RID: 1015 RVA: 0x00010D94 File Offset: 0x0000EF94
		[JsonProperty("height")]
		public int? Height { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00010D9D File Offset: 0x0000EF9D
		// (set) Token: 0x060003F9 RID: 1017 RVA: 0x00010DA5 File Offset: 0x0000EFA5
		[JsonProperty("width")]
		public int? Width { get; set; }

		// Token: 0x060003FA RID: 1018 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedImage()
		{
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00010DAE File Offset: 0x0000EFAE
		public EmbedImage(string url, int? height = null, int? width = null, string proxyUrl = null)
		{
			this.Url = url;
			this.ProxyUrl = proxyUrl;
			this.Height = height;
			this.Width = width;
		}
	}
}
