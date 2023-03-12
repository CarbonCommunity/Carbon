/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Messages.AllowedMentions
{
	// Token: 0x02000074 RID: 116
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum AllowedMentionTypes
	{
		// Token: 0x0400028A RID: 650
		Unknown,
		// Token: 0x0400028B RID: 651
		[System.ComponentModel.Description("roles")]
		Roles,
		// Token: 0x0400028C RID: 652
		[System.ComponentModel.Description("users")]
		Users,
		// Token: 0x0400028D RID: 653
		[System.ComponentModel.Description("everyone")]
		Everyone
	}
}
