/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.AuditLogs.Change
{
	// Token: 0x02000116 RID: 278
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class AuditLogChangeUser
	{
		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000A3D RID: 2621 RVA: 0x0001783C File Offset: 0x00015A3C
		// (set) Token: 0x06000A3E RID: 2622 RVA: 0x00017844 File Offset: 0x00015A44
		[JsonProperty("deaf")]
		public bool Deaf { get; set; }

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000A3F RID: 2623 RVA: 0x0001784D File Offset: 0x00015A4D
		// (set) Token: 0x06000A40 RID: 2624 RVA: 0x00017855 File Offset: 0x00015A55
		[JsonProperty("mute")]
		public bool Mute { get; set; }

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000A41 RID: 2625 RVA: 0x0001785E File Offset: 0x00015A5E
		// (set) Token: 0x06000A42 RID: 2626 RVA: 0x00017866 File Offset: 0x00015A66
		[JsonProperty("nick")]
		public string Nick { get; set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x0001786F File Offset: 0x00015A6F
		// (set) Token: 0x06000A44 RID: 2628 RVA: 0x00017877 File Offset: 0x00015A77
		[JsonProperty("avatar_hash")]
		public string AvatarHash { get; set; }
	}
}
