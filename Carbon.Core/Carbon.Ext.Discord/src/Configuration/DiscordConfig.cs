/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Newtonsoft.Json;
using Oxide.Core.Configuration;

namespace Oxide.Ext.Discord.Configuration
{
	// Token: 0x02000129 RID: 297
	public class DiscordConfig : ConfigFile
	{
		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x00018090 File Offset: 0x00016290
		// (set) Token: 0x06000ADB RID: 2779 RVA: 0x00018098 File Offset: 0x00016298
		[JsonProperty("Commands")]
		public DiscordCommandsConfig Commands { get; set; }

		// Token: 0x06000ADC RID: 2780 RVA: 0x000180A1 File Offset: 0x000162A1
		public DiscordConfig(string filename) : base(filename)
		{
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x000180AC File Offset: 0x000162AC
		public override void Load(string filename = null)
		{
			try
			{
				base.Load(filename);
				DiscordCommandsConfig discordCommandsConfig = new DiscordCommandsConfig();
				DiscordCommandsConfig discordCommandsConfig2 = discordCommandsConfig;
				DiscordCommandsConfig commands = this.Commands;
				char[] commandPrefixes;
				if ((commandPrefixes = ((commands != null) ? commands.CommandPrefixes : null)) == null)
				{
					char[] array = new char[2];
					array[0] = '/';
					commandPrefixes = array;
					array[1] = '!';
				}
				discordCommandsConfig2.CommandPrefixes = commandPrefixes;
				this.Commands = discordCommandsConfig;
			}
			catch (Exception arg)
			{
				DiscordExtension.GlobalLogger.Error(string.Format("Failed to load config file. Generating new Config.\n{0}", arg));
				this.Commands = new DiscordCommandsConfig
				{
					CommandPrefixes = new char[]
					{
						'/',
						'!'
					}
				};
				this.Save(filename);
			}
		}
	}
}
