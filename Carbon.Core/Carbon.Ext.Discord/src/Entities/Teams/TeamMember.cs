/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Teams
{
	// Token: 0x02000054 RID: 84
	[JsonObject(MemberSerialization = (MemberSerialization)1)]
	public class TeamMember
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000F3BE File Offset: 0x0000D5BE
		// (set) Token: 0x0600029E RID: 670 RVA: 0x0000F3C6 File Offset: 0x0000D5C6
		[JsonProperty("membership_state")]
		public TeamMembershipState MembershipState { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600029F RID: 671 RVA: 0x0000F3CF File Offset: 0x0000D5CF
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x0000F3D7 File Offset: 0x0000D5D7
		[JsonProperty("permissions")]
		public List<string> Permissions { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000F3E0 File Offset: 0x0000D5E0
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0000F3E8 File Offset: 0x0000D5E8
		[JsonProperty("team_id")]
		public Snowflake TeamId { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000F3F1 File Offset: 0x0000D5F1
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000F3F9 File Offset: 0x0000D5F9
		[JsonProperty("user")]
		public DiscordUser User { get; set; }
	}
}
