/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	// Token: 0x0200009C RID: 156
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class IntegrationApplication
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x000123E2 File Offset: 0x000105E2
		// (set) Token: 0x0600057C RID: 1404 RVA: 0x000123EA File Offset: 0x000105EA
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x000123F3 File Offset: 0x000105F3
		// (set) Token: 0x0600057E RID: 1406 RVA: 0x000123FB File Offset: 0x000105FB
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x00012404 File Offset: 0x00010604
		// (set) Token: 0x06000580 RID: 1408 RVA: 0x0001240C File Offset: 0x0001060C
		[JsonProperty("icon")]
		public string Icon { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x00012415 File Offset: 0x00010615
		// (set) Token: 0x06000582 RID: 1410 RVA: 0x0001241D File Offset: 0x0001061D
		[JsonProperty("description")]
		public string Description { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x00012426 File Offset: 0x00010626
		// (set) Token: 0x06000584 RID: 1412 RVA: 0x0001242E File Offset: 0x0001062E
		[JsonProperty("summary")]
		public string Summary { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x00012437 File Offset: 0x00010637
		// (set) Token: 0x06000586 RID: 1414 RVA: 0x0001243F File Offset: 0x0001063F
		[JsonProperty("bot")]
		public DiscordUser Bot { get; set; }
	}
}
