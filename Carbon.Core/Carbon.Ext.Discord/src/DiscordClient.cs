/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Attributes;
using Oxide.Ext.Discord.Entities.Gatway;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord
{
	// Token: 0x02000003 RID: 3
	public class DiscordClient
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002A2B File Offset: 0x00000C2B
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002A33 File Offset: 0x00000C33
		public Plugin Owner { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002A3C File Offset: 0x00000C3C
		public List<Plugin> RegisteredForHooks { get; } = new List<Plugin>();

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002A44 File Offset: 0x00000C44
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002A4C File Offset: 0x00000C4C
		public BotClient Bot { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002A55 File Offset: 0x00000C55
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002A5D File Offset: 0x00000C5D
		public DiscordSettings Settings { get; private set; }

		// Token: 0x06000025 RID: 37 RVA: 0x00002A66 File Offset: 0x00000C66
		public DiscordClient(Plugin plugin)
		{
			this.Owner = plugin;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002A84 File Offset: 0x00000C84
		public void Connect(string apiKey, GatewayIntents intents)
		{
			DiscordSettings settings = new DiscordSettings
			{
				ApiToken = apiKey,
				LogLevel = DiscordLogLevel.Info,
				Intents = intents
			};
			this.Connect(settings);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002AB8 File Offset: 0x00000CB8
		public void Connect(DiscordSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			this.Settings = settings;
			this.Logger = new Logger(settings.LogLevel);
			bool flag = string.IsNullOrEmpty(this.Settings.ApiToken);
			if (flag)
			{
				this.Logger.Error("API Token is null or empty!");
			}
			else
			{
				bool flag2 = !DiscordClient.TokenValidator.IsMatch(this.Settings.ApiToken);
				if (flag2)
				{
					this.Logger.Warning("API Token does not appear to be a valid discord bot token: " + this.Settings.GetHiddenToken() + ". Please confirm you are using the correct bot token. If the token is correct and this message is showing please let the Discord Extension Developers know.");
				}
				bool flag3 = !string.IsNullOrEmpty("");
				if (flag3)
				{
					this.Logger.Warning("Using Discord Test Version: ");
				}
				this.Logger.Debug("DiscordClient.Connect GetOrCreate bot for " + this.Owner.Name);
				this.Bot = BotClient.GetOrCreate(this);
				this.RegisterPluginForHooks(this.Owner);
				Interface.Call("OnDiscordClientConnected", new object[]
				{
					this.Owner,
					this
				});
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002BE4 File Offset: 0x00000DE4
		public void Disconnect()
		{
			Interface.Call("OnDiscordClientDisconnected", new object[]
			{
				this.Owner,
				this
			});
			BotClient bot = this.Bot;
			if (bot != null)
			{
				bot.RemoveClient(this);
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002C18 File Offset: 0x00000E18
		public void RegisterPluginForHooks(Plugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			this.RemovePluginFromHooks(plugin);
			this.RegisteredForHooks.Add(plugin);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002C50 File Offset: 0x00000E50
		public void RemovePluginFromHooks(Plugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			this.RegisteredForHooks.RemoveAll((Plugin p) => p.Name == plugin.Name);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002C9C File Offset: 0x00000E9C
		public void CallHook(string hookName, params object[] args)
		{
			Interface.Oxide.NextTick(delegate()
			{
				for (int i = 0; i < this.RegisteredForHooks.Count; i++)
				{
					Plugin plugin = this.RegisteredForHooks[i];
					plugin.CallHook(hookName, args);
				}
			});
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002CDC File Offset: 0x00000EDC
		public static void CreateClient(RustPlugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			DiscordClient.OnPluginAdded(plugin);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002D08 File Offset: 0x00000F08
		public static DiscordClient GetClient(Plugin plugin)
		{
			bool flag = plugin == null;
			if (flag)
			{
				throw new ArgumentNullException("plugin");
			}
			return DiscordClient.GetClient(plugin.Name);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002D3C File Offset: 0x00000F3C
		public static DiscordClient GetClient(string pluginName)
		{
			return DiscordClient.Clients[pluginName];
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002D5C File Offset: 0x00000F5C
		internal static void OnPluginAdded(RustPlugin plugin)
		{
			DiscordClient.OnPluginRemoved(plugin);
			foreach (FieldInfo fieldInfo in plugin.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				bool flag = fieldInfo.GetCustomAttributes(typeof(DiscordClientAttribute), true).Length != 0;
				if (flag)
				{
					DiscordClient discordClient = DiscordClient.Clients[plugin.Name];
					bool flag2 = discordClient == null;
					if (flag2)
					{
						DiscordExtension.GlobalLogger.Debug("DiscordClient.OnPluginAdded Creating DiscordClient for plugin " + plugin.Name);
						discordClient = new DiscordClient(plugin);
						DiscordClient.Clients[plugin.Name] = discordClient;
					}
					fieldInfo.SetValue(plugin, discordClient);
					plugin.Call("OnDiscordClientCreated");
					break;
				}
			}
			DiscordExtension.DiscordCommand.ProcessPluginCommands(plugin);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002E34 File Offset: 0x00001034
		internal static void OnPluginRemoved(RustPlugin plugin)
		{
			DiscordClient discordClient = DiscordClient.Clients[plugin.Name];
			bool flag = discordClient == null;
			if (!flag)
			{
				DiscordClient.CloseClient(discordClient);
				foreach (DiscordClient discordClient2 in DiscordClient.Clients.Values)
				{
					discordClient2.RemovePluginFromHooks(plugin);
				}
				DiscordExtension.DiscordLink.OnPluginUnloaded(plugin);
				DiscordExtension.DiscordCommand.OnPluginUnloaded(plugin);
				DiscordExtension.DiscordSubscriptions.OnPluginUnloaded(plugin);
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002ED4 File Offset: 0x000010D4
		internal static void CloseClient(DiscordClient client)
		{
			bool flag = client == null;
			if (!flag)
			{
				client.Disconnect();
				DiscordExtension.GlobalLogger.Debug("DiscordClient.CloseClient Closing DiscordClient for plugin " + client.Owner.Name);
				foreach (FieldInfo fieldInfo in client.Owner.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					bool flag2 = fieldInfo.GetCustomAttributes(typeof(DiscordClientAttribute), true).Length != 0;
					if (flag2)
					{
						fieldInfo.SetValue(client.Owner, null);
						break;
					}
				}
				DiscordClient.Clients.Remove(client.Owner.Name);
				client.Owner = null;
				client.RegisteredForHooks.Clear();
			}
		}

		// Token: 0x0400000E RID: 14
		internal static readonly Hash<string, DiscordClient> Clients = new Hash<string, DiscordClient>();

		// Token: 0x0400000F RID: 15
		private static readonly Regex TokenValidator = new Regex("^[\\w-]+\\.[\\w-]+\\.[\\w-]+$", RegexOptions.Compiled);

		// Token: 0x04000014 RID: 20
		internal ILogger Logger;
	}
}
