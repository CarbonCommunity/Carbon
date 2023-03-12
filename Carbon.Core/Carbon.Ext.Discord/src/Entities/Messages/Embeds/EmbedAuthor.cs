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
	// Token: 0x0200006C RID: 108
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedAuthor
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x00010C51 File Offset: 0x0000EE51
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x00010C59 File Offset: 0x0000EE59
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060003DA RID: 986 RVA: 0x00010C62 File Offset: 0x0000EE62
		// (set) Token: 0x060003DB RID: 987 RVA: 0x00010C6A File Offset: 0x0000EE6A
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060003DC RID: 988 RVA: 0x00010C73 File Offset: 0x0000EE73
		// (set) Token: 0x060003DD RID: 989 RVA: 0x00010C7B File Offset: 0x0000EE7B
		[JsonProperty("icon_url")]
		public string IconUrl { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060003DE RID: 990 RVA: 0x00010C84 File Offset: 0x0000EE84
		// (set) Token: 0x060003DF RID: 991 RVA: 0x00010C8C File Offset: 0x0000EE8C
		[JsonProperty("proxy_icon_url")]
		public string ProxyIconUrl { get; set; }

		// Token: 0x060003E0 RID: 992 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedAuthor()
		{
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00010C95 File Offset: 0x0000EE95
		public EmbedAuthor(string name, string url = null, string iconUrl = null, string proxyIconUrl = null)
		{
			this.Name = name;
			this.Url = url;
			this.IconUrl = iconUrl;
			this.ProxyIconUrl = proxyIconUrl;
		}
	}
}
