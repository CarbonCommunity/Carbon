/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
	// Token: 0x02000021 RID: 33
	internal class GuildCommand : BaseCommand
	{
		// Token: 0x0600014C RID: 332 RVA: 0x0000BED3 File Offset: 0x0000A0D3
		public GuildCommand(string name, Plugin plugin, List<Snowflake> allowedChannels, Action<DiscordMessage, string, string[]> callback) : base(name, plugin, callback)
		{
			this._allowedChannels = (allowedChannels ?? new List<Snowflake>());
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
		public override bool CanHandle(DiscordMessage message, DiscordChannel channel)
		{
			bool flag = message.GuildId == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._allowedChannels.Count != 0 && !this._allowedChannels.Contains(channel.Id) && (channel.ParentId == null || !this._allowedChannels.Contains(channel.ParentId.Value));
				result = !flag2;
			}
			return result;
		}

		// Token: 0x040000EC RID: 236
		private readonly List<Snowflake> _allowedChannels;
	}
}
