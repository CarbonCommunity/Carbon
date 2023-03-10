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
	// Token: 0x02000070 RID: 112
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedProvider
	{
		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x00010DD9 File Offset: 0x0000EFD9
		// (set) Token: 0x060003FD RID: 1021 RVA: 0x00010DE1 File Offset: 0x0000EFE1
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x00010DEA File Offset: 0x0000EFEA
		// (set) Token: 0x060003FF RID: 1023 RVA: 0x00010DF2 File Offset: 0x0000EFF2
		[JsonProperty("url")]
		public string Url { get; set; }

		// Token: 0x06000400 RID: 1024 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedProvider()
		{
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00010DFB File Offset: 0x0000EFFB
		public EmbedProvider(string name, string url)
		{
			this.Name = name;
			this.Url = url;
		}
	}
}
