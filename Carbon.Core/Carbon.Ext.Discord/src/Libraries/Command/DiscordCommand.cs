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
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Attributes;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
	// Token: 0x02000020 RID: 32
	public class DiscordCommand : Library
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600013A RID: 314 RVA: 0x0000B664 File Offset: 0x00009864
		private Lang Lang
		{
			get
			{
				bool flag = this._lang != null;
				Lang result;
				if (flag)
				{
					result = this._lang;
				}
				else
				{
					result = (this._lang = Interface.Oxide.GetLibrary<Lang>(null));
				}
				return result;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000B6A1 File Offset: 0x000098A1
		public DiscordCommand(char[] prefixes)
		{
			this.CommandPrefixes = prefixes;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000B6C8 File Offset: 0x000098C8
		public bool HasCommands()
		{
			return this.HasDirectMessageCommands() || this.HasGuildCommands();
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000B6EC File Offset: 0x000098EC
		public bool HasDirectMessageCommands()
		{
			return this._directMessageCommands.Count != 0;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000B70C File Offset: 0x0000990C
		public bool HasGuildCommands()
		{
			return this._guildCommands.Count != 0;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000B72C File Offset: 0x0000992C
		public void AddDirectMessageCommand(string name, Plugin plugin, string callback)
		{
			this.AddDirectMessageCommand(name, plugin, delegate(DiscordMessage message, string command, string[] args)
			{
				plugin.CallHook(callback, new object[]
				{
					message,
					command,
					args
				});
			});
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000B768 File Offset: 0x00009968
		public void AddDirectMessageLocalizedCommand(string langKey, RustPlugin plugin, string callback)
		{
			foreach (string text in this.Lang.GetLanguages(plugin))
			{
				Dictionary<string, string> messages = this.Lang.GetMessages(text, plugin);
				string text2;
				bool flag = messages.TryGetValue(langKey, out text2) && !string.IsNullOrEmpty(text2);
				if (flag)
				{
					this.AddDirectMessageCommand(text2, plugin, callback);
				}
			}
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000B7D4 File Offset: 0x000099D4
		public void AddDirectMessageCommand(string command, Plugin plugin, Action<DiscordMessage, string, string[]> callback)
		{
			string text = command.ToLowerInvariant();
			DirectMessageCommand directMessageCommand;
			bool flag = this._directMessageCommands.TryGetValue(text, out directMessageCommand);
			if (flag)
			{
				Plugin plugin2 = directMessageCommand.Plugin;
				string text2 = ((plugin2 != null) ? plugin2.Name : null) ?? "an unknown plugin";
				string text3 = ((plugin != null) ? plugin.Name : null) ?? "An unknown plugin";
				DiscordExtension.GlobalLogger.Warning(string.Concat(new string[]
				{
					text3,
					" has replaced the '",
					text,
					"' discord direct message command previously registered by ",
					text2
				}));
			}
			directMessageCommand = new DirectMessageCommand(text, plugin, callback);
			this._directMessageCommands[text] = directMessageCommand;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000B87C File Offset: 0x00009A7C
		public void AddGuildCommand(string name, Plugin plugin, List<Snowflake> allowedChannels, string callback)
		{
			this.AddGuildCommand(name, plugin, allowedChannels, delegate(DiscordMessage message, string command, string[] args)
			{
				plugin.CallHook(callback, new object[]
				{
					message,
					command,
					args
				});
			});
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000B8BC File Offset: 0x00009ABC
		public void AddGuildLocalizedCommand(string langKey, RustPlugin plugin, List<Snowflake> allowedChannels, string callback)
		{
			foreach (string text in this.Lang.GetLanguages(plugin))
			{
				Dictionary<string, string> messages = this.Lang.GetMessages(text, plugin);
				string text2;
				bool flag = messages.TryGetValue(langKey, out text2) && !string.IsNullOrEmpty(text2);
				if (flag)
				{
					this.AddGuildCommand(text2, plugin, allowedChannels, callback);
				}
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000B928 File Offset: 0x00009B28
		public void AddGuildCommand(string command, Plugin plugin, List<Snowflake> allowedChannels, Action<DiscordMessage, string, string[]> callback)
		{
			string text = command.ToLowerInvariant();
			GuildCommand guildCommand;
			bool flag = this._guildCommands.TryGetValue(text, out guildCommand);
			if (flag)
			{
				Plugin plugin2 = guildCommand.Plugin;
				string text2 = ((plugin2 != null) ? plugin2.Name : null) ?? "an unknown plugin";
				string text3 = ((plugin != null) ? plugin.Name : null) ?? "An unknown plugin";
				DiscordExtension.GlobalLogger.Warning(string.Concat(new string[]
				{
					text3,
					" has replaced the '",
					text,
					"' discord guild command previously registered by ",
					text2
				}));
			}
			guildCommand = new GuildCommand(text, plugin, allowedChannels, callback);
			this._guildCommands[text] = guildCommand;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000B9D0 File Offset: 0x00009BD0
		public void RemoveDiscordCommand(string command, Plugin plugin)
		{
			DirectMessageCommand directMessageCommand = this._directMessageCommands[command];
			bool flag = directMessageCommand != null && directMessageCommand.Plugin == plugin;
			if (flag)
			{
				this.RemoveDmCommand(directMessageCommand);
			}
			GuildCommand guildCommand = this._guildCommands[command];
			bool flag2 = guildCommand != null && guildCommand.Plugin == plugin;
			if (flag2)
			{
				this.RemoveGuildCommand(guildCommand);
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000BA34 File Offset: 0x00009C34
		private void RemoveDmCommand(DirectMessageCommand command)
		{
			DirectMessageCommand directMessageCommand = this._directMessageCommands[command.Name];
			directMessageCommand.OnRemoved();
			this._directMessageCommands.Remove(command.Name);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000BA70 File Offset: 0x00009C70
		private void RemoveGuildCommand(GuildCommand command)
		{
			GuildCommand guildCommand = this._guildCommands[command.Name];
			guildCommand.OnRemoved();
			this._guildCommands.Remove(command.Name);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000BAAC File Offset: 0x00009CAC
		internal void OnPluginUnloaded(Plugin sender)
		{
			List<DirectMessageCommand> list = new List<DirectMessageCommand>();
			List<GuildCommand> list2 = new List<GuildCommand>();
			foreach (DirectMessageCommand directMessageCommand in this._directMessageCommands.Values)
			{
				bool flag = directMessageCommand.Plugin.Name == sender.Name;
				if (flag)
				{
					list.Add(directMessageCommand);
				}
			}
			foreach (GuildCommand guildCommand in this._guildCommands.Values)
			{
				bool flag2 = guildCommand.Plugin.Name == sender.Name;
				if (flag2)
				{
					list2.Add(guildCommand);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				DirectMessageCommand command = list[i];
				this.RemoveDmCommand(command);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				GuildCommand command2 = list2[j];
				this.RemoveGuildCommand(command2);
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000BBFC File Offset: 0x00009DFC
		internal bool HandleDirectMessageCommand(BotClient client, DiscordMessage message, DiscordChannel channel, string name, string[] args)
		{
			DirectMessageCommand directMessageCommand = this._directMessageCommands[name];
			bool flag = directMessageCommand == null || !directMessageCommand.CanRun(client) || !directMessageCommand.CanHandle(message, channel);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !directMessageCommand.Plugin.IsLoaded;
				if (flag2)
				{
					this._directMessageCommands.Remove(name);
					result = false;
				}
				else
				{
					bool flag3 = !client.IsPluginRegistered(directMessageCommand.Plugin);
					if (flag3)
					{
						result = false;
					}
					else
					{
						directMessageCommand.HandleCommand(message, name, args);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000BC8C File Offset: 0x00009E8C
		internal bool HandleGuildCommand(BotClient client, DiscordMessage message, DiscordChannel channel, string name, string[] args)
		{
			GuildCommand guildCommand = this._guildCommands[name];
			bool flag = guildCommand == null || !guildCommand.CanRun(client) || !guildCommand.CanHandle(message, channel);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !guildCommand.Plugin.IsLoaded;
				if (flag2)
				{
					this._guildCommands.Remove(name);
					result = false;
				}
				else
				{
					bool flag3 = !client.IsPluginRegistered(guildCommand.Plugin);
					if (flag3)
					{
						result = false;
					}
					else
					{
						guildCommand.HandleCommand(message, name, args);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000BD1C File Offset: 0x00009F1C
		internal void ProcessPluginCommands(RustPlugin plugin)
		{
			foreach (MethodInfo methodInfo in plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(DirectMessageCommandAttribute), true);
				bool flag = customAttributes.Length != 0;
				if (flag)
				{
					DirectMessageCommandAttribute directMessageCommandAttribute = (DirectMessageCommandAttribute)customAttributes[0];
					bool isLocalized = directMessageCommandAttribute.IsLocalized;
					if (isLocalized)
					{
						DiscordExtension.DiscordCommand.AddDirectMessageLocalizedCommand(directMessageCommandAttribute.Name, plugin, methodInfo.Name);
						DiscordExtension.GlobalLogger.Debug("Adding Localized Direct Message Command " + directMessageCommandAttribute.Name + " Method: " + methodInfo.Name);
					}
					else
					{
						DiscordExtension.DiscordCommand.AddDirectMessageCommand(directMessageCommandAttribute.Name, plugin, methodInfo.Name);
						DiscordExtension.GlobalLogger.Debug("Adding Direct Message Command " + directMessageCommandAttribute.Name + " Method: " + methodInfo.Name);
					}
				}
				customAttributes = methodInfo.GetCustomAttributes(typeof(GuildCommandAttribute), true);
				bool flag2 = customAttributes.Length != 0;
				if (flag2)
				{
					GuildCommandAttribute guildCommandAttribute = (GuildCommandAttribute)customAttributes[0];
					bool isLocalized2 = guildCommandAttribute.IsLocalized;
					if (isLocalized2)
					{
						DiscordExtension.DiscordCommand.AddGuildLocalizedCommand(guildCommandAttribute.Name, plugin, null, methodInfo.Name);
						DiscordExtension.GlobalLogger.Debug("Adding Localized Guild Command " + guildCommandAttribute.Name + " Method: " + methodInfo.Name);
					}
					else
					{
						DiscordExtension.DiscordCommand.AddGuildCommand(guildCommandAttribute.Name, plugin, null, methodInfo.Name);
						DiscordExtension.GlobalLogger.Debug("Adding Guild Command " + guildCommandAttribute.Name + " Method: " + methodInfo.Name);
					}
				}
			}
		}

		// Token: 0x040000E8 RID: 232
		public readonly char[] CommandPrefixes;

		// Token: 0x040000E9 RID: 233
		private readonly Hash<string, DirectMessageCommand> _directMessageCommands = new Hash<string, DirectMessageCommand>();

		// Token: 0x040000EA RID: 234
		private readonly Hash<string, GuildCommand> _guildCommands = new Hash<string, GuildCommand>();

		// Token: 0x040000EB RID: 235
		private Lang _lang;
	}
}
