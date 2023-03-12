/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Text.RegularExpressions;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Logging;

namespace Oxide.Ext.Discord
{
	// Token: 0x02000005 RID: 5
	public class DiscordSettings
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003244 File Offset: 0x00001444
		public string GetHiddenToken()
		{
			return "\"" + DiscordSettings.HideTokenRegex.Replace(this.ApiToken, "#") + "\"";
		}

		// Token: 0x0400001D RID: 29
		public string ApiToken;

		// Token: 0x0400001E RID: 30
		public DiscordLogLevel LogLevel = DiscordLogLevel.Info;

		// Token: 0x0400001F RID: 31
		public GatewayIntents Intents = GatewayIntents.None;

		// Token: 0x04000020 RID: 32
		private static readonly Regex HideTokenRegex = new Regex("\\w|-", RegexOptions.Compiled);
	}
}
