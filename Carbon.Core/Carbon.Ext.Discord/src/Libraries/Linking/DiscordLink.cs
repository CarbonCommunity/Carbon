/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Carbon.Extensions;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Linking
{
	// Token: 0x0200001C RID: 28
	public class DiscordLink : Library
	{
		// Token: 0x06000118 RID: 280 RVA: 0x0000ADBC File Offset: 0x00008FBC
		public DiscordLink(ILogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000AE1C File Offset: 0x0000901C
		public bool IsEnabled()
		{
			return this._linkPlugins.Count != 0;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000AE3C File Offset: 0x0000903C
		public void AddLinkPlugin(IDiscordLinkPlugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			bool flag2 = !this._linkPlugins.Contains(plugin);
			if (flag2)
			{
				this._linkPlugins.Add(plugin);
			}
			IDictionary<string, Snowflake> steamToDiscordIds = plugin.GetSteamToDiscordIds();
			bool flag3 = steamToDiscordIds == null;
			if (flag3)
			{
				this._logger.Error(plugin.Title + " returned null when GetSteamToDiscordIds was called");
			}
			else
			{
				this._pluginLinks[plugin.Title] = steamToDiscordIds;
				foreach (KeyValuePair<string, Snowflake> keyValuePair in steamToDiscordIds)
				{
					this._steamIdToDiscordId[keyValuePair.Key] = keyValuePair.Value;
					this._discordIdToSteamId[keyValuePair.Value] = keyValuePair.Key;
					this._steamIds.Add(keyValuePair.Key);
					this._discordIds.Add(keyValuePair.Value);
				}
				this._logger.Debug(plugin.Title + " registered as a DiscordLink plugin");
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000AF78 File Offset: 0x00009178
		public void RemoveLinkPlugin(IDiscordLinkPlugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			IDictionary<string, Snowflake> dictionary = this._pluginLinks[plugin.Title];
			bool flag2 = dictionary != null;
			if (flag2)
			{
				foreach (KeyValuePair<string, Snowflake> keyValuePair in dictionary)
				{
					this._steamIdToDiscordId.Remove(keyValuePair.Key);
					this._discordIdToSteamId.Remove(keyValuePair.Value);
					this._steamIds.Remove(keyValuePair.Key);
					this._discordIds.Remove(keyValuePair.Value);
				}
			}
			this._linkPlugins.Remove(plugin);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000B048 File Offset: 0x00009248
		internal void OnPluginUnloaded(Plugin plugin)
		{
			IDiscordLinkPlugin discordLinkPlugin = plugin as IDiscordLinkPlugin;
			bool flag = discordLinkPlugin != null;
			if (flag)
			{
				this.RemoveLinkPlugin(discordLinkPlugin);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000B070 File Offset: 0x00009270
		public bool IsLinked(string steamId)
		{
			Hash<string, Snowflake> steamToDiscordIds = this.GetSteamToDiscordIds();
			return steamToDiscordIds != null && steamToDiscordIds.ContainsKey(steamId);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000B098 File Offset: 0x00009298
		public bool IsLinked(Snowflake discordId)
		{
			Hash<Snowflake, string> discordToSteamIds = this.GetDiscordToSteamIds();
			return discordToSteamIds != null && discordToSteamIds.ContainsKey(discordId);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000B0C0 File Offset: 0x000092C0
		public bool IsLinked(IPlayer player)
		{
			return this.IsLinked(player.Id);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000B0E0 File Offset: 0x000092E0
		public bool IsLinked(DiscordUser user)
		{
			return this.IsLinked(user.Id);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000B100 File Offset: 0x00009300
		public string GetSteamId(Snowflake discordId)
		{
			Hash<Snowflake, string> discordToSteamIds = this.GetDiscordToSteamIds();
			return (discordToSteamIds != null) ? discordToSteamIds[discordId] : null;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000B128 File Offset: 0x00009328
		public string GetSteamId(DiscordUser user)
		{
			return this.GetSteamId(user.Id);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000B148 File Offset: 0x00009348
		public IPlayer GetPlayer(Snowflake discordId)
		{
			string steamId = this.GetSteamId(discordId);
			bool flag = string.IsNullOrEmpty(steamId);
			IPlayer result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = BasePlayer.FindByID(steamId.ToUlong()).AsIPlayer();
			}
			return result;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000B180 File Offset: 0x00009380
		public Snowflake? GetDiscordId(string steamId)
		{
			Hash<string, Snowflake> steamToDiscordIds = this.GetSteamToDiscordIds();
			return (steamToDiscordIds != null) ? new Snowflake?(steamToDiscordIds[steamId]) : null;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000B1B4 File Offset: 0x000093B4
		public Snowflake? GetDiscordId(IPlayer player)
		{
			return this.GetDiscordId(player.Id);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000B1D4 File Offset: 0x000093D4
		public DiscordUser GetDiscordUser(string steamId)
		{
			Snowflake? discordId = this.GetDiscordId(steamId);
			bool flag = discordId == null;
			DiscordUser result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new DiscordUser
				{
					Id = discordId.Value,
					Bot = new bool?(false)
				};
			}
			return result;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000B224 File Offset: 0x00009424
		public DiscordUser GetDiscordUser(IPlayer player)
		{
			return this.GetDiscordUser(player.Id);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000B244 File Offset: 0x00009444
		public GuildMember GetLinkedMember(string steamId, DiscordGuild guild)
		{
			Snowflake? discordId = this.GetDiscordId(steamId);
			bool flag = discordId == null || !guild.IsAvailable;
			GuildMember result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = guild.Members[discordId.Value];
			}
			return result;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000B290 File Offset: 0x00009490
		public GuildMember GetLinkedMember(IPlayer player, DiscordGuild guild)
		{
			return this.GetLinkedMember(player.Id, guild);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000B2B0 File Offset: 0x000094B0
		public int GetLinkedCount()
		{
			Hash<string, Snowflake> steamToDiscordIds = this.GetSteamToDiscordIds();
			return (steamToDiscordIds != null) ? steamToDiscordIds.Count : 0;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000B2D4 File Offset: 0x000094D4
		public HashSet<string> GetSteamIds()
		{
			return this._steamIds;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000B2EC File Offset: 0x000094EC
		public HashSet<Snowflake> GetDiscordIds()
		{
			return this._discordIds;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000B304 File Offset: 0x00009504
		public Hash<string, Snowflake> GetSteamToDiscordIds()
		{
			return this._steamIdToDiscordId;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000B31C File Offset: 0x0000951C
		public Hash<Snowflake, string> GetDiscordToSteamIds()
		{
			return this._discordIdToSteamId;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000B334 File Offset: 0x00009534
		public void OnLinked(Plugin plugin, IPlayer player, DiscordUser discord)
		{
			bool flag = player == null;
			if (flag)
			{
				throw new ArgumentNullException("player");
			}
			bool flag2 = discord == null;
			if (flag2)
			{
				throw new ArgumentNullException("discord");
			}
			IDiscordLinkPlugin discordLinkPlugin = plugin as IDiscordLinkPlugin;
			bool flag3 = discordLinkPlugin == null;
			if (flag3)
			{
				this._logger.Error(plugin.Name + " tried to link but is not registered as a link plugin");
			}
			bool flag4 = !this._linkPlugins.Contains(discordLinkPlugin);
			if (flag4)
			{
				this._logger.Error(plugin.Name + " has not been added as a link plugin and cannot set a link");
			}
			else
			{
				this._pluginLinks[plugin.Title][player.Id] = discord.Id;
				this._discordIdToSteamId[discord.Id] = player.Id;
				this._steamIdToDiscordId[player.Id] = discord.Id;
				this._steamIds.Add(player.Id);
				this._discordIds.Add(discord.Id);
				Interface.Oxide.CallHook("OnDiscordPlayerLinked", new object[]
				{
					player,
					discord
				});
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000B464 File Offset: 0x00009664
		public void OnUnlinked(Plugin plugin, IPlayer player, DiscordUser discord)
		{
			bool flag = player == null;
			if (flag)
			{
				throw new ArgumentNullException("player");
			}
			bool flag2 = discord == null;
			if (flag2)
			{
				throw new ArgumentNullException("discord");
			}
			IDiscordLinkPlugin discordLinkPlugin = plugin as IDiscordLinkPlugin;
			bool flag3 = discordLinkPlugin == null;
			if (flag3)
			{
				this._logger.Error(plugin.Name + " tried to unlink but is not registered as a link plugin");
			}
			bool flag4 = !this._linkPlugins.Contains(discordLinkPlugin);
			if (flag4)
			{
				this._logger.Error(plugin.Name + " has not been added as a link plugin and cannot unlink");
			}
			else
			{
				this._pluginLinks[plugin.Title].Remove(player.Id);
				this._discordIdToSteamId.Remove(discord.Id);
				this._steamIdToDiscordId.Remove(player.Id);
				this._steamIds.Remove(player.Id);
				this._discordIds.Remove(discord.Id);
				Interface.Oxide.CallHook("OnDiscordPlayerUnlinked", new object[]
				{
					player,
					discord
				});
			}
		}

		// Token: 0x040000DE RID: 222
		private readonly Hash<string, Snowflake> _steamIdToDiscordId = new Hash<string, Snowflake>();

		// Token: 0x040000DF RID: 223
		private readonly Hash<Snowflake, string> _discordIdToSteamId = new Hash<Snowflake, string>();

		// Token: 0x040000E0 RID: 224
		private readonly HashSet<string> _steamIds = new HashSet<string>();

		// Token: 0x040000E1 RID: 225
		private readonly HashSet<Snowflake> _discordIds = new HashSet<Snowflake>();

		// Token: 0x040000E2 RID: 226
		private readonly Hash<string, IDictionary<string, Snowflake>> _pluginLinks = new Hash<string, IDictionary<string, Snowflake>>();

		// Token: 0x040000E3 RID: 227
		private readonly List<IDiscordLinkPlugin> _linkPlugins = new List<IDiscordLinkPlugin>();

		// Token: 0x040000E4 RID: 228
		private readonly ILogger _logger;
	}
}
