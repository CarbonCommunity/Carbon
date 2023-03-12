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
	// Token: 0x0200006E RID: 110
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedFooter
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x00010D15 File Offset: 0x0000EF15
		// (set) Token: 0x060003EB RID: 1003 RVA: 0x00010D1D File Offset: 0x0000EF1D
		[JsonProperty("text")]
		public string Text { get; set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x00010D26 File Offset: 0x0000EF26
		// (set) Token: 0x060003ED RID: 1005 RVA: 0x00010D2E File Offset: 0x0000EF2E
		[JsonProperty("icon_url")]
		public string IconUrl { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x00010D37 File Offset: 0x0000EF37
		// (set) Token: 0x060003EF RID: 1007 RVA: 0x00010D3F File Offset: 0x0000EF3F
		[JsonProperty("proxy_icon_url")]
		public string ProxyIconUrl { get; set; }

		// Token: 0x060003F0 RID: 1008 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedFooter()
		{
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00010D48 File Offset: 0x0000EF48
		public EmbedFooter(string text, string iconUrl = null, string proxyIconUrl = null)
		{
			this.Text = text;
			this.IconUrl = iconUrl;
			this.ProxyIconUrl = proxyIconUrl;
		}
	}
}
