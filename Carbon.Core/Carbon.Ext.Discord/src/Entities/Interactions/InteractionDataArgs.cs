/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Interactions
{
	// Token: 0x0200007F RID: 127
	public class InteractionDataArgs
	{
		// Token: 0x060004B1 RID: 1201 RVA: 0x000118B0 File Offset: 0x0000FAB0
		internal InteractionDataArgs(DiscordInteraction interaction, List<InteractionDataOption> options)
		{
			this._interaction = interaction;
			for (int i = 0; i < options.Count; i++)
			{
				InteractionDataOption interactionDataOption = options[i];
				this._args[interactionDataOption.Name] = interactionDataOption;
			}
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001190C File Offset: 0x0000FB0C
		public bool HasArg(string name)
		{
			return this._args.ContainsKey(name);
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001192C File Offset: 0x0000FB2C
		public string GetString(string name, string @default = null)
		{
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.String);
			return ((string)((arg != null) ? arg.Value : null)) ?? @default;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001195C File Offset: 0x0000FB5C
		public int GetInt(string name, int @default = 0)
		{
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.Integer);
			return ((int?)((arg != null) ? arg.Value : null)) ?? @default;
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0001199C File Offset: 0x0000FB9C
		public bool GetBool(string name, bool @default = false)
		{
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.Boolean);
			return ((bool?)((arg != null) ? arg.Value : null)) ?? @default;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000119DC File Offset: 0x0000FBDC
		public DiscordUser GetUser(string name)
		{
			Hash<Snowflake, DiscordUser> users = this._interaction.Data.Resolved.Users;
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.User);
			return users[this.GetEntityId((arg != null) ? arg.Value : null)];
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00011A24 File Offset: 0x0000FC24
		public DiscordChannel GetChannel(string name)
		{
			Hash<Snowflake, DiscordChannel> channels = this._interaction.Data.Resolved.Channels;
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.Channel);
			return channels[this.GetEntityId((arg != null) ? arg.Value : null)];
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00011A6C File Offset: 0x0000FC6C
		public DiscordRole GetRole(string name)
		{
			Hash<Snowflake, DiscordRole> roles = this._interaction.Data.Resolved.Roles;
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.Role);
			return roles[this.GetEntityId((arg != null) ? arg.Value : null)];
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x00011AB4 File Offset: 0x0000FCB4
		public double GetNumber(string name, double @default)
		{
			InteractionDataOption arg = this.GetArg(name, CommandOptionType.Number);
			return ((double?)((arg != null) ? arg.Value : null)) ?? @default;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00011AF4 File Offset: 0x0000FCF4
		private InteractionDataOption GetArg(string name, CommandOptionType requested)
		{
			InteractionDataOption interactionDataOption = this._args[name];
			bool flag = interactionDataOption == null;
			InteractionDataOption result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = interactionDataOption.Type != CommandOptionType.Mentionable;
				if (flag2)
				{
					this.ValidateArgs(name, interactionDataOption.Type, requested);
				}
				else
				{
					bool flag3 = requested != CommandOptionType.Role && requested != CommandOptionType.User;
					if (flag3)
					{
						throw new Exception(string.Format("Attempted to parse {0} role/user type to: {1} which is not valid.", name, requested));
					}
				}
				result = interactionDataOption;
			}
			return result;
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x00011B74 File Offset: 0x0000FD74
		private void ValidateArgs(string name, CommandOptionType arg, CommandOptionType requested)
		{
			bool flag = arg != requested;
			if (flag)
			{
				throw new Exception(string.Format("Attempted to parse {0} {1} type to: {2} which is not valid.", name, arg, requested));
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00011BAC File Offset: 0x0000FDAC
		private Snowflake GetEntityId(JToken value)
		{
			bool flag = value == null;
			Snowflake result;
			if (flag)
			{
				result = default(Snowflake);
			}
			else
			{
				result = value.ToObject<Snowflake>();
			}
			return result;
		}

		// Token: 0x040002D8 RID: 728
		private readonly Hash<string, InteractionDataOption> _args = new Hash<string, InteractionDataOption>();

		// Token: 0x040002D9 RID: 729
		private readonly DiscordInteraction _interaction;
	}
}
