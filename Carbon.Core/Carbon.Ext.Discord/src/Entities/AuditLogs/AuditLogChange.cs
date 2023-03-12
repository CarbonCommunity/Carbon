/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.AuditLogs.Change;

namespace Oxide.Ext.Discord.Entities.AuditLogs
{
	// Token: 0x0200010D RID: 269
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChange
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060009BA RID: 2490 RVA: 0x0001742F File Offset: 0x0001562F
		// (set) Token: 0x060009BB RID: 2491 RVA: 0x00017437 File Offset: 0x00015637
		[JsonProperty("new_value")]
		public AuditLogChangeBase NewValue { get; set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060009BC RID: 2492 RVA: 0x00017440 File Offset: 0x00015640
		// (set) Token: 0x060009BD RID: 2493 RVA: 0x00017448 File Offset: 0x00015648
		[JsonProperty("old_value")]
		public AuditLogChangeBase OldValue { get; set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060009BE RID: 2494 RVA: 0x00017451 File Offset: 0x00015651
		// (set) Token: 0x060009BF RID: 2495 RVA: 0x00017459 File Offset: 0x00015659
		[JsonProperty("key")]
		public string Key { get; set; }
	}
}
