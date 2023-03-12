/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities;

namespace Oxide.Ext.Discord.Libraries.Linking
{
	// Token: 0x0200001D RID: 29
	public interface IDiscordLinkPlugin
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000131 RID: 305
		string Title { get; }

		// Token: 0x06000132 RID: 306
		IDictionary<string, Snowflake> GetSteamToDiscordIds();
	}
}
