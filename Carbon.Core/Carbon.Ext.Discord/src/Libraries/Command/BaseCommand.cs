/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
	// Token: 0x0200001E RID: 30
	internal class BaseCommand
	{
		// Token: 0x06000133 RID: 307 RVA: 0x0000B57F File Offset: 0x0000977F
		protected BaseCommand(string name, Plugin plugin, Action<DiscordMessage, string, string[]> callback)
		{
			this.Name = name;
			this.Plugin = plugin;
			this._callback = callback;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000B5A0 File Offset: 0x000097A0
		public void HandleCommand(DiscordMessage message, string name, string[] args)
		{
			Interface.Oxide.NextTick(delegate()
			{
				try
				{
					this.Plugin.TrackStart();
					this._callback(message, name, args);
					this.Plugin.TrackEnd();
				}
				catch (Exception ex)
				{
					ILogger globalLogger = DiscordExtension.GlobalLogger;
					string str = "An exception occured in discord command ";
					string name2 = name;
					string str2 = " for plugin ";
					Plugin plugin = this.Plugin;
					globalLogger.Exception(str + name2 + str2 + ((plugin != null) ? plugin.Name : null), ex);
				}
			});
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000B5E8 File Offset: 0x000097E8
		public bool CanRun(BotClient client)
		{
			bool result;
			if (client != null)
			{
				DiscordClient discordClient = DiscordClient.Clients[this.Plugin.Name];
				result = (((discordClient != null) ? discordClient.Bot : null) == client);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000B624 File Offset: 0x00009824
		public virtual bool CanHandle(DiscordMessage message, DiscordChannel channel)
		{
			return true;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000B627 File Offset: 0x00009827
		internal void OnRemoved()
		{
			this.Plugin = null;
		}

		// Token: 0x040000E5 RID: 229
		internal readonly string Name;

		// Token: 0x040000E6 RID: 230
		internal Plugin Plugin;

		// Token: 0x040000E7 RID: 231
		private readonly Action<DiscordMessage, string, string[]> _callback;
	}
}
