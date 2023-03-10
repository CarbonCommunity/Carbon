/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	// Token: 0x0200009B RID: 155
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class Account
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x000123C0 File Offset: 0x000105C0
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x000123C8 File Offset: 0x000105C8
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x000123D1 File Offset: 0x000105D1
		// (set) Token: 0x06000579 RID: 1401 RVA: 0x000123D9 File Offset: 0x000105D9
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
