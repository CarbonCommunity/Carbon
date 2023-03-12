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
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	// Token: 0x0200009A RID: 154
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class Integration : IntegrationUpdate, ISnowflakeEntity
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x000122EB File Offset: 0x000104EB
		// (set) Token: 0x0600055E RID: 1374 RVA: 0x000122F3 File Offset: 0x000104F3
		[JsonProperty("id")]
		public Snowflake Id { get; set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x000122FC File Offset: 0x000104FC
		// (set) Token: 0x06000560 RID: 1376 RVA: 0x00012304 File Offset: 0x00010504
		[JsonProperty("name")]
		public string Name { get; set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000561 RID: 1377 RVA: 0x0001230D File Offset: 0x0001050D
		// (set) Token: 0x06000562 RID: 1378 RVA: 0x00012315 File Offset: 0x00010515
		[JsonProperty("type")]
		public IntegrationType Type { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000563 RID: 1379 RVA: 0x0001231E File Offset: 0x0001051E
		// (set) Token: 0x06000564 RID: 1380 RVA: 0x00012326 File Offset: 0x00010526
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x0001232F File Offset: 0x0001052F
		// (set) Token: 0x06000566 RID: 1382 RVA: 0x00012337 File Offset: 0x00010537
		[JsonProperty("syncing")]
		public bool? Syncing { get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00012340 File Offset: 0x00010540
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x00012348 File Offset: 0x00010548
		[JsonProperty("role_id")]
		public Snowflake? RoleId { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00012351 File Offset: 0x00010551
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x00012359 File Offset: 0x00010559
		[JsonProperty("user")]
		public DiscordUser User { get; set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x00012362 File Offset: 0x00010562
		// (set) Token: 0x0600056C RID: 1388 RVA: 0x0001236A File Offset: 0x0001056A
		[JsonProperty("account")]
		public Account Account { get; set; }

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00012373 File Offset: 0x00010573
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x0001237B File Offset: 0x0001057B
		[JsonProperty("synced_at")]
		public DateTime? SyncedAt { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00012384 File Offset: 0x00010584
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x0001238C File Offset: 0x0001058C
		[JsonProperty("subscriber_count")]
		public int? SubscriberCount { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x00012395 File Offset: 0x00010595
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x0001239D File Offset: 0x0001059D
		[JsonProperty("revoked")]
		public bool? Revoked { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x000123A6 File Offset: 0x000105A6
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x000123AE File Offset: 0x000105AE
		[JsonProperty("application")]
		public IntegrationApplication Application { get; set; }
	}
}
