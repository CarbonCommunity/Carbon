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

namespace Oxide.Ext.Discord.Entities.Users.Connections
{
	// Token: 0x02000051 RID: 81
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum ConnectionType
	{
		// Token: 0x04000176 RID: 374
		Unknown,
		// Token: 0x04000177 RID: 375
		[System.ComponentModel.Description("twitch")]
		Twitch,
		// Token: 0x04000178 RID: 376
		[System.ComponentModel.Description("youtube")]
		Youtube
	}
}
