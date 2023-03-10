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
	// Token: 0x0200006D RID: 109
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class EmbedField
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00010CC0 File Offset: 0x0000EEC0
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x00010CC8 File Offset: 0x0000EEC8
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x00010CD1 File Offset: 0x0000EED1
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x00010CD9 File Offset: 0x0000EED9
		[JsonProperty("value")]
		public string Value { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x00010CE2 File Offset: 0x0000EEE2
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x00010CEA File Offset: 0x0000EEEA
		[JsonProperty("inline")]
		public bool Inline { get; set; }

		// Token: 0x060003E8 RID: 1000 RVA: 0x00010A8D File Offset: 0x0000EC8D
		public EmbedField()
		{
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00010CF3 File Offset: 0x0000EEF3
		public EmbedField(string name, string value, bool inline)
		{
			this.Name = name;
			this.Value = value;
			this.Inline = inline;
		}
	}
}
