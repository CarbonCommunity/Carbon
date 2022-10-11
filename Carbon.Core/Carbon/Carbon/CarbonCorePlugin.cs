///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Carbon.Core.Extensions;
using Carbon.Core.Modules;
using Facepunch;
using Carbon.Components;
using Carbon.Extensions;
using Newtonsoft.Json;
using Oxide.Plugins;
using UnityEngine;
using System.Reflection;

namespace Carbon.Core
{
	public class CarbonCorePlugin : RustPlugin
	{
		public static Dictionary<string, string> OrderedFiles { get; } = new Dictionary<string, string>();

		public static void RefreshOrderedFiles()
		{
			OrderedFiles.Clear();

			foreach (var file in OsEx.Folder.GetFilesWithExtension(CarbonDefines.GetPluginsFolder(), "cs"))
			{
				OrderedFiles.Add(Path.GetFileNameWithoutExtension(file), file);
			}
		}

		public static string GetPluginPath(string shortName)
		{
			foreach (var file in OrderedFiles)
			{
				if (file.Key == shortName) return file.Value;
			}

			return null;
		}

		public override void IInit()
		{
			foreach (var player in BasePlayer.activePlayerList)
			{
				permission.RefreshUser(player);
			}

			CarbonCore.Instance.ModuleProcessor.Init();

			timer.Every(5f, () =>
			{
				if (!Logger._hasInit || Logger._buffer.Count == 0 || CarbonCore.Instance.Config.LogFileMode != 1) return;

				Logger._flush();
			});
		}

		private void OnPluginLoaded(Plugin plugin)
		{
		}
		private void OnPluginUnloaded(Plugin plugin)
		{
		}
		private void OnPlayerConnected(BasePlayer player)
		{
			permission.RefreshUser(player);
		}
		private void OnEntitySpawned(BaseEntity entity)
		{
			Entities.AddMap(entity);
		}
		private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
		{
			Entities.RemoveMap(entity);
		}
		private void OnEntityKill(BaseEntity entity)
		{
			Entities.RemoveMap(entity);
		}

		internal static void Reply(object message, ConsoleSystem.Arg arg)
		{
			if (arg != null && arg.Player() != null)
			{
				arg.Player().SendConsoleCommand($"echo {message}");
				return;
			}
			Carbon.Logger.Log(message);
		}

		[ConsoleCommand("version", "Returns currently loaded version of Carbon.")]
		private void GetVersion(ConsoleSystem.Arg arg)
		{
			Reply($"Carbon v{CarbonCore.Version}", arg);
		}

		[ConsoleCommand("build", "Returns current version of Carbon's Assembly.")]
		private void GetBuild(ConsoleSystem.Arg arg)
		{
			Reply($"{CarbonCore.InformationalVersion}", arg);
		}

		[ConsoleCommand("plugins", "Prints the list of mods and their loaded plugins.")]
		private void Plugins(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			var mode = arg.HasArgs(1) ? arg.Args[0] : null;

			switch (mode)
			{
				case "-j":
				case "--j":
				case "-json":
				case "--json":
					Reply(JsonConvert.SerializeObject(CarbonLoader._loadedMods, Formatting.Indented), arg);
					break;

				default:
					var body = new StringTable("#", "Mod", "Author", "Version", "Core", "Hook Time", "Compile Time");
					var count = 1;

					foreach (var mod in CarbonLoader._loadedMods)
					{
						body.AddRow($"{count:n0}", $"{mod.Name}{(mod.Plugins.Count > 1 ? $" ({mod.Plugins.Count:n0})" : "")}", "", "", mod.IsCoreMod ? "Yes" : "No", "", "");

						foreach (var plugin in mod.Plugins)
						{
							body.AddRow($"", plugin.Name, plugin.Author, $"v{plugin.Version}", plugin.IsCorePlugin ? "Yes" : "No", $"{plugin.TotalHookTime * 1000f:0.0}ms", $"{plugin.CompileTime * 1000f:0.0}ms");
						}

						count++;
					}

					Reply(body.ToStringMinimal(), arg);
					break;
			}
		}

		[ConsoleCommand("hooks", "Prints the list of all hooks that have been called at least once.")]
		private void HookInfo(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			var body = new StringTable("#", "Hook", "Current Time", "Total Time", "Plugins Using");
			var count = 1;

			foreach (var mod in CarbonCore.Instance.HookProcessor.Patches)
			{
				if (!CarbonCore.Instance.HookProcessor.Patches.TryGetValue(mod.Key, out var instance))
				{
					continue;
				}

				body.AddRow($"{count:n0}", mod.Key, $"{HookExecutor.GetHookTime(mod.Key)}ms", $"{HookExecutor.GetHookTotalTime(mod.Key)}ms", $"{instance.Hooks}");
				count++;
			}

			Reply(body.ToStringMinimal(), arg);
		}

		[ConsoleCommand("webload")]
		private void WebLoad(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			CarbonCore.Instance.WebScriptProcessor.Prepare(arg.Args[0]);
		}

		#region Config

		[ConsoleCommand("loadconfig", "Loads Carbon config from file.")]
		private void CarbonLoadConfig(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || CarbonCore.Instance == null) return;

			CarbonCore.Instance.LoadConfig();

			Reply("Loaded Carbon config.", arg);
		}

		[ConsoleCommand("saveconfig", "Saves Carbon config to file.")]
		private void CarbonSaveConfig(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || CarbonCore.Instance == null) return;

			CarbonCore.Instance.SaveConfig();

			Reply("Saved Carbon config.", arg);
		}

		[CommandVar("modding", "Mark this server as modded or not.", true)]
		private bool Modding { get { return CarbonCore.Instance.Config.IsModded; } set { CarbonCore.Instance.Config.IsModded = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("tag", "Displays this server in the browser list with the 'carbon' tag.", true)]
		private bool CarbonTag { get { return CarbonCore.Instance.Config.CarbonTag; } set { CarbonCore.Instance.Config.CarbonTag = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("debug", "The level of debug logging for Carbon. Helpful for very detailed logs in case things break. (Set it to -1 to disable debug logging.)", true)]
		private int CarbonDebug { get { return CarbonCore.Instance.Config.LogVerbosity; } set { CarbonCore.Instance.Config.LogVerbosity = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("logfiletype", "The mode for writing the log to file. (0=disabled, 1=saves updates every 5 seconds, 2=saves immediately)", true)]
		private int LogFileType { get { return CarbonCore.Instance.Config.LogFileMode; } set { CarbonCore.Instance.Config.LogFileMode = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("hooktimetracker", "For debugging purposes, this will track the time of hooks and gives a total.", true)]
		private bool HookTimeTracker { get { return CarbonCore.Instance.Config.HookTimeTracker; } set { CarbonCore.Instance.Config.HookTimeTracker = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("hookvalidation", "Prints a warning when plugins contain Oxide hooks that aren't available yet in Carbon.", true)]
		private bool HookValidation { get { return CarbonCore.Instance.Config.HookValidation; } set { CarbonCore.Instance.Config.HookValidation = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("entitymapbuffersize", "The entity map buffer size. Gets applied on Carbon reboot.", true)]
		private int EntityMapBufferSize { get { return CarbonCore.Instance.Config.EntityMapBufferSize; } set { CarbonCore.Instance.Config.EntityMapBufferSize = value; CarbonCore.Instance.SaveConfig(); } }

		[CommandVar("language", "Server language used by the Language API.", true)]
		private string Language { get { return CarbonCore.Instance.Config.Language; } set { CarbonCore.Instance.Config.Language = value; CarbonCore.Instance.SaveConfig(); } }

		#endregion

		#region Commands

		[ConsoleCommand("find", "Searches through Carbon-processed console commands.")]
		private void Find(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			var body = new StringBody();
			var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args[0] : null;
			body.Add($"Console Commands:");

			foreach (var command in CarbonCore.Instance.AllConsoleCommands)
			{
				if (!string.IsNullOrEmpty(filter) && !command.Command.Contains(filter)) continue;

				var reference = " ";

				if (command.Reference != null)
				{
					if (command.Reference is FieldInfo field) reference = field.GetValue(command.Plugin)?.ToString();
					else if (command.Reference is PropertyInfo property) reference = property.GetValue(command.Plugin)?.ToString();
				}

				body.Add($" {command.Command}( {reference} )  {command.Help}");
			}

			Reply(body.ToNewLine(), arg);
		}

		[ConsoleCommand("findchat", "Searches through Carbon-processed chat commands.")]
		private void FindChat(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			var body = new StringBody();
			var filter = arg.Args != null && arg.Args.Length > 0 ? arg.Args[0] : null;
			body.Add($"Chat Commands:");

			foreach (var command in CarbonCore.Instance.AllChatCommands)
			{
				if (!string.IsNullOrEmpty(filter) && !command.Command.Contains(filter)) continue;

				body.Add($" {command.Command}(   )  {command.Help}");
			}

			Reply(body.ToNewLine(), arg);
		}

		#endregion

		#region Report

		[ConsoleCommand("report", "Reloads all current plugins, and returns a report based on them at the output path.")]
		private void Report(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			new Report().Init();
		}

		#endregion

		#region Modules

		[ConsoleCommand("setmodule", "Enables or disables Carbon modules. Visit root/carbon/modules and use the config file names as IDs.")]
		private void SetModule(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(2)) return;

			var hookable = CarbonCore.Instance.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
			var module = hookable.To<IModule>();

			if (module == null)
			{
				Reply($"Couldn't find that module.", arg);
				return;
			}

			var previousEnabled = module.GetEnabled();
			var newEnabled = arg.Args[1].ToBool();

			if (previousEnabled != newEnabled)
			{
				module.SetEnabled(newEnabled);
				module.Save();
			}

			Reply($"{module.Name} marked {(module.GetEnabled() ? "enabled" : "disabled")}.", arg);
		}

		[ConsoleCommand("saveallmodules", "Saves the configs and data files of all available modules.")]
		private void SaveAllModules(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			foreach (var hookable in CarbonCore.Instance.ModuleProcessor.Modules)
			{
				var module = hookable.To<IModule>();
				module.Save();
			}

			Reply($"Saved {CarbonCore.Instance.ModuleProcessor.Modules.Count:n0} module configs and data files.", arg);
		}

		[ConsoleCommand("savemoduleconfig", "Saves Carbon module config & data file.")]
		private void SaveModuleConfig(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			var hookable = CarbonCore.Instance.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
			var module = hookable.To<IModule>();

			if (module == null)
			{
				Reply($"Couldn't find that module.", arg);
				return;
			}

			module.Save();

			Reply($"Saved '{module.Name}' module config & data file.", arg);
		}

		[ConsoleCommand("loadmoduleconfig", "Loads Carbon module config & data file.")]
		private void LoadModuleConfig(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			var hookable = CarbonCore.Instance.ModuleProcessor.Modules.FirstOrDefault(x => x.Name == arg.Args[0]);
			var module = hookable.To<IModule>();

			if (module == null)
			{
				Reply($"Couldn't find that module.", arg);
				return;
			}

			if (module.GetEnabled()) module.SetEnabled(false);
			module.Load();
			if (module.GetEnabled()) module.OnEnableStatus();

			Reply($"Reloaded '{module.Name}' module config.", arg);
		}

		#endregion

		#region Mod & Plugin Loading

		[ConsoleCommand("reload", "Reloads all or specific mods / plugins. E.g 'c.reload *' to reload everything.")]
		private void Reload(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			RefreshOrderedFiles();

			var name = arg.Args[0];
			switch (name)
			{
				case "*":
					CarbonCore.ClearPlugins();
					CarbonCore.ReloadPlugins();
					break;

				default:
					var path = GetPluginPath(name);

					if (!string.IsNullOrEmpty(path))
					{
						CarbonCore.Instance.HarmonyProcessor.Prepare(name, path);
						CarbonCore.Instance.ScriptProcessor.Prepare(name, path);
						return;
					}

					foreach (var mod in CarbonLoader._loadedMods)
					{
						var plugins = Pool.GetList<RustPlugin>();
						plugins.AddRange(mod.Plugins);

						foreach (var plugin in plugins)
						{
							if (plugin.Name == name)
							{
								plugin._processor_instance.Dispose();
								plugin._processor_instance.Execute();
								mod.Plugins.Remove(plugin);
							}
						}

						Pool.FreeList(ref plugins);
					}
					break;
			}
		}

		[ConsoleCommand("load", "Loads all mods and/or plugins. E.g 'c.load *' to load everything you've unloaded.")]
		private void LoadPlugin(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			RefreshOrderedFiles();

			var name = arg.Args[0];
			switch (name)
			{
				case "*":
					//
					// Mods
					//
					{
						var tempList = Pool.GetList<string>();
						tempList.AddRange(CarbonCore.Instance.HarmonyProcessor.IgnoreList);
						CarbonCore.Instance.HarmonyProcessor.IgnoreList.Clear();

						foreach (var plugin in tempList)
						{
							CarbonCore.Instance.HarmonyProcessor.Prepare(plugin, plugin);
						}
						Pool.FreeList(ref tempList);
					}

					//
					// Scripts
					//
					{
						var tempList = Pool.GetList<string>();
						tempList.AddRange(CarbonCore.Instance.ScriptProcessor.IgnoreList);
						CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear();

						foreach (var plugin in tempList)
						{
							CarbonCore.Instance.ScriptProcessor.Prepare(plugin, plugin);
						}
						Pool.FreeList(ref tempList);
					}
					break;

				default:
					var path = GetPluginPath(name);
					if (!string.IsNullOrEmpty(path))
					{
						CarbonCore.Instance.HarmonyProcessor.ClearIgnore(path);
						CarbonCore.Instance.ScriptProcessor.ClearIgnore(path);

						CarbonCore.Instance.HarmonyProcessor.Prepare(path);
						CarbonCore.Instance.ScriptProcessor.Prepare(path);
						return;
					}

					var module = BaseModule.GetModule<DRMModule>();
					foreach (var drm in module.Config.DRMs)
					{
						foreach (var entry in drm.Entries)
						{
							if (entry.Id == name) drm.RequestEntry(entry);
						}
					}

					break;
			}
		}

		[ConsoleCommand("unload", "Unloads all mods and/or plugins. E.g 'c.unload *' to unload everything. They'll be marked as 'ignored'.")]
		private void UnloadPlugin(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin() || !arg.HasArgs(1)) return;

			RefreshOrderedFiles();

			var name = arg.Args[0];
			switch (name)
			{
				case "*":
					//
					// Mods
					//
					{
						foreach (var plugin in CarbonCore.Instance.HarmonyProcessor.InstanceBuffer)
						{
							CarbonCore.Instance.HarmonyProcessor.Ignore(plugin.Value.File);
						}
						CarbonCore.Instance.HarmonyProcessor.Clear();
					}

					//
					// Scripts
					//
					{
						var tempList = Pool.GetList<string>();
						tempList.AddRange(CarbonCore.Instance.ScriptProcessor.IgnoreList);
						CarbonCore.Instance.ScriptProcessor.IgnoreList.Clear();
						CarbonCore.Instance.ScriptProcessor.Clear();

						foreach (var plugin in tempList)
						{
							CarbonCore.Instance.ScriptProcessor.Ignore(plugin);
						}
						Pool.FreeList(ref tempList);
					}

					//
					// Web-Scripts
					//
					{
						var tempList = Pool.GetList<string>();
						tempList.AddRange(CarbonCore.Instance.WebScriptProcessor.IgnoreList);
						CarbonCore.Instance.WebScriptProcessor.IgnoreList.Clear();
						CarbonCore.Instance.WebScriptProcessor.Clear();

						foreach (var plugin in tempList)
						{
							CarbonCore.Instance.WebScriptProcessor.Ignore(plugin);
						}
						Pool.FreeList(ref tempList);
					}
					break;

				default:
					var path = GetPluginPath(name);
					if (!string.IsNullOrEmpty(path))
					{
						CarbonCore.Instance.HarmonyProcessor.Ignore(path);
						CarbonCore.Instance.ScriptProcessor.Ignore(path);
						CarbonCore.Instance.WebScriptProcessor.Ignore(path);
					}

					foreach (var mod in CarbonLoader._loadedMods)
					{
						var plugins = Pool.GetList<RustPlugin>();
						plugins.AddRange(mod.Plugins);

						foreach (var plugin in plugins)
						{
							if (plugin.Name == name)
							{
								plugin._processor_instance.Dispose();
								mod.Plugins.Remove(plugin);
							}
						}

						Pool.FreeList(ref plugins);
					}
					break;
			}
		}

		#endregion

		#region Permissions

		[ConsoleCommand("grant", "Grant one or more permissions to users or groups. Do 'c.grant' for syntax info.")]
		private void Grant(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			void PrintWarn()
			{
				Reply($"Syntax: c.grant <user|group> <name|id> <perm>", arg);
			}

			if (!arg.HasArgs(3))
			{
				PrintWarn();
				return;
			}

			var action = arg.Args[0];
			var name = arg.Args[1];
			var perm = arg.Args[2];
			var user = permission.FindUser(name);

			switch (action)
			{
				case "user":
					if (permission.GrantUserPermission(user.Key, perm, null))
					{
						Reply($"Granted user '{user.Value.LastSeenNickname}' permission '{perm}'", arg);
					}
					break;

				case "group":
					if (permission.GrantGroupPermission(name, perm, null))
					{
						Reply($"Granted group '{name}' permission '{perm}'", arg);
					}
					break;

				default:
					PrintWarn();
					break;
			}
		}

		[ConsoleCommand("revoke", "Revoke one or more permissions from users or groups. Do 'c.revoke' for syntax info.")]
		private void Revoke(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			void PrintWarn()
			{
				Reply($"Syntax: c.revoke <user|group> <name|id> <perm>", arg);
			}

			if (!arg.HasArgs(3))
			{
				PrintWarn();
				return;
			}

			var action = arg.Args[0];
			var name = arg.Args[1];
			var perm = arg.Args[2];
			var user = permission.FindUser(name);

			switch (action)
			{
				case "user":
					if (permission.RevokeUserPermission(user.Key, perm))
					{
						Reply($"Revoked user '{user.Value?.LastSeenNickname}' permission '{perm}'", arg);
					}
					break;

				case "group":
					if (permission.RevokeGroupPermission(name, perm))
					{
						Reply($"Revoked group '{name}' permission '{perm}'", arg);
					}
					break;

				default:
					PrintWarn();
					break;
			}
		}

		[ConsoleCommand("show", "Displays information about a specific player or group (incl. permissions, groups and user list). Do 'c.show' for syntax info.")]
		private void Show(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			void PrintWarn()
			{
				Reply($"Syntax: c.show <user|group> <name|id>", arg);
			}

			if (!arg.HasArgs(2))
			{
				PrintWarn();
				return;
			}

			var action = arg.Args[0];
			var name = arg.Args[1];

			switch (action)
			{
				case "user":
					var user = permission.FindUser(name);
					if (user.Value == null)
					{
						Reply($"Couldn't find that user.", arg);
						return;
					}

					Reply($"User {user.Value.LastSeenNickname}[{user.Key}] found in {user.Value.Groups.Count:n0} groups:\n  {user.Value.Groups.Select(x => x).ToArray().ToString(", ", " and ")}", arg);
					Reply($"and has {user.Value.Perms.Count:n0} permissions:\n  {user.Value.Perms.Select(x => x).ToArray().ToString(", ", " and ")}", arg);
					break;

				case "group":
					if (!permission.GroupExists(name))
					{
						Reply($"Couldn't find that group.", arg);
						return;
					}

					var users = permission.GetUsersInGroup(name);
					var permissions = permission.GetGroupPermissions(name, false);
					Reply($"Group {name} has {users.Length:n0} users:\n  {users.Select(x => x).ToArray().ToString(", ", " and ")}", arg);
					Reply($"and has {permissions.Length:n0} permissions:\n  {permissions.Select(x => x).ToArray().ToString(", ", " and ")}", arg);
					break;

				default:
					PrintWarn();
					break;
			}
		}

		[ConsoleCommand("usergroup", "Adds or removes a player from a group. Do 'c.usergroup' for syntax info.")]
		private void UserGroup(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			void PrintWarn()
			{
				Reply($"Syntax: c.usergroup <add|remove> <player> <group>", arg);
			}

			if (!arg.HasArgs(3))
			{
				PrintWarn();
				return;
			}

			var action = arg.Args[0];
			var player = arg.Args[1];
			var group = arg.Args[2];

			var user = permission.FindUser(player);

			if (user.Value == null)
			{
				Reply($"Couldn't find that player.", arg);
				return;
			}

			if (!permission.GroupExists(group))
			{
				Reply($"Group '{group}' could not be found.", arg);
				return;
			}

			switch (action)
			{
				case "add":
					if (permission.UserHasGroup(user.Key, group))
					{
						Reply($"{user.Value.LastSeenNickname}[{user.Key}] is already in '{group}' group.", arg);
						return;
					}

					permission.AddUserGroup(user.Key, group);
					Reply($"Added {user.Value.LastSeenNickname}[{user.Key}] to '{group}' group.", arg);
					break;

				case "remove":
					if (!permission.UserHasGroup(user.Key, group))
					{
						Reply($"{user.Value.LastSeenNickname}[{user.Key}] isn't in '{group}' group.", arg);
						return;
					}

					permission.RemoveUserGroup(user.Key, group);
					Reply($"Removed {user.Value.LastSeenNickname}[{user.Key}] from '{group}' group.", arg);
					break;

				default:
					PrintWarn();
					break;
			}
		}

		[ConsoleCommand("group", "Adds or removes a group. Do 'c.group' for syntax info.")]
		private void Group(ConsoleSystem.Arg arg)
		{
			if (!arg.IsPlayerCalledAndAdmin()) return;

			void PrintWarn()
			{
				Reply($"Syntax: c.group add <group> [<displayName>] [<rank>]", arg);
				Reply($"Syntax: c.group remove <group>", arg);
				Reply($"Syntax: c.group set <group> <title|rank> <value>", arg);
				Reply($"Syntax: c.group parent <group> [<parent>]", arg);
			}

			if (!arg.HasArgs(1)) { PrintWarn(); return; }

			var action = arg.Args[0];

			switch (action)
			{
				case "add":
					{
						if (!arg.HasArgs(2)) { PrintWarn(); return; }

						var group = arg.Args[1];

						if (permission.GroupExists(group))
						{
							Reply($"Group '{group}' already exists. To set any values for this group, use 'c.group set'.", arg);
							return;
						}

						if (permission.CreateGroup(group, arg.HasArgs(3) ? arg.Args[2] : group, arg.HasArgs(4) ? arg.Args[3].ToInt() : 0))
						{
							Reply($"Created '{group}' group.", arg);
						}
					}
					break;

				case "set":
					{
						if (!arg.HasArgs(2)) { PrintWarn(); return; }

						var group = arg.Args[1];

						if (!permission.GroupExists(group))
						{
							Reply($"Group '{group}' does not exists.", arg);
							return;
						}

						if (arg.HasArgs(3)) permission.SetGroupTitle(group, arg.Args[2]);
						if (arg.HasArgs(4)) permission.SetGroupTitle(group, arg.Args[3]);

						Reply($"Set '{group}' group.", arg);
					}
					break;
				case "remove":
					{
						if (!arg.HasArgs(2)) { PrintWarn(); return; }

						var group = arg.Args[1];

						if (permission.RemoveGroup(group)) Reply($"Removed '{group}' group.", arg);
						else Reply($"Couldn't remove '{group}' group.", arg);
					}
					break;

				default:
					PrintWarn();
					break;
			}
		}

		#endregion
	}
}
