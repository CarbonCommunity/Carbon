/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Guilds
{
	// Token: 0x020000A2 RID: 162
	public enum ExplicitContentFilterLevel
	{
		// Token: 0x040003A4 RID: 932
		[System.ComponentModel.Description("DISABLED")]
		Disabled,
		// Token: 0x040003A5 RID: 933
		[System.ComponentModel.Description("MEMBERS_WITHOUT_ROLES")]
		MembersWithoutRoles,
		// Token: 0x040003A6 RID: 934
		[System.ComponentModel.Description("ALL_MEMBERS")]
		AllMembers
	}
}
