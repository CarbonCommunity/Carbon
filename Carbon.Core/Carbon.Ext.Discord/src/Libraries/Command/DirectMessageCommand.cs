/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
	// Token: 0x0200001F RID: 31
	internal class DirectMessageCommand : BaseCommand
	{
		// Token: 0x06000138 RID: 312 RVA: 0x0000B631 File Offset: 0x00009831
		internal DirectMessageCommand(string name, Plugin plugin, Action<DiscordMessage, string, string[]> callback) : base(name, plugin, callback)
		{
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000B640 File Offset: 0x00009840
		public override bool CanHandle(DiscordMessage message, DiscordChannel channel)
		{
			return message.GuildId == null;
		}
	}
}
