using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Carbon;
using Carbon.Base;
using Facepunch;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries.Covalence;
using Oxide.Plugins;
using static ConsoleSystem;
using Pool = Facepunch.Pool;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust.Libraries
{
	public class Command : Library
	{
		public static bool FromRcon { get; set; }

		public void AddChatCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			if (Community.Runtime.AllChatCommands.Count(x => x.Command == command) == 0)
			{
				Community.Runtime.AllChatCommands.Add(new OxideCommand
				{
					Command = command,
					Plugin = plugin,
					SkipOriginal = skipOriginal,
					Callback = (player, cmd, args) =>
					{
						try { callback.Invoke(player, cmd, args); }
						catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }
					},
					Help = help,
					Reference = reference,
					Permissions = permissions,
					Groups = groups,
					AuthLevel = authLevel,
					Cooldown = cooldown,
					IsHidden = isHidden,
					Protected = @protected
				});
			}
			else if(!silent) Logger.Warn($"Chat command '{command}' already exists.");
		}
		public void AddChatCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			AddChatCommand(command, plugin, (player, cmd, args) =>
			{
				var arguments = Pool.GetList<object>();
				var result = (object[])null;
				try
				{
					var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer)));
					var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer)));
					var methodInfo = covalenceMethod ?? consoleMethod;
					var parameters = methodInfo.GetParameters();

					if (parameters.Length > 0)
					{
						if (methodInfo == covalenceMethod)
						{
							var iplayer = player.AsIPlayer();
							iplayer.IsServer = player == null;
							arguments.Add(iplayer);
						}
						else arguments.Add(player);

						switch (parameters.Length)
						{
							case 2:
								{
									arguments.Add(cmd);
									break;
								}

							case 3:
						 		{
									arguments.Add(cmd);
									arguments.Add(args);
									break;
								}
						}

						var currentArgumentCount = arguments.Count;

						if (parameters.Length > currentArgumentCount)
						{
							for (int i = 0; i < parameters.Length - currentArgumentCount; i++)
							{
								arguments.Add(null);
							}
						}
					}

					result = arguments.ToArray();

					methodInfo?.Invoke(plugin, result);
				}
				catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }

				if (arguments != null) Pool.FreeList(ref arguments);
				if (result != null) Pool.Free(ref result);
			}, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			if (Community.Runtime.AllConsoleCommands.Count(x => x.Command == command) == 0)
			{
				Community.Runtime.AllConsoleCommands.Add(new OxideCommand
				{
					Command = command,
					Plugin = plugin,
					SkipOriginal = skipOriginal,
					Callback = callback,
					Help = help,
					Reference = reference,
					Permissions = permissions,
					Groups = groups,
					AuthLevel = authLevel,
					Cooldown = cooldown,
					IsHidden = isHidden,
					Protected = @protected
				});
			}
			else if(!silent) Logger.Warn($"Console command '{command}' already exists.");
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			AddConsoleCommand(command, plugin, (player, cmd, args) =>
			{
				var arguments = Pool.GetList<object>();
				var result = (object[])null;

				try
				{
					var fullString = args == null || args.Length == 0 ? string.Empty : string.Join(" ", args);
					var client = player == null ? Option.Unrestricted : Option.Client;
					var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
					if (player != null) client = client.FromConnection(player.net.connection);
					client.FromRcon = FromRcon;
					arg.Option = client;
					arg.FullString = fullString;
					arg.Args = args;

					try
					{
						var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer)));
						var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer)));
						var methodInfo = covalenceMethod ?? consoleMethod;
						var parameters = methodInfo.GetParameters();

						if (parameters.Length > 0)
						{
							if (methodInfo == covalenceMethod)
							{
								if (player == null) arguments.Add(new RustPlayer { IsServer = true });
								else
								{
									var iplayer = player.AsIPlayer();
									iplayer.IsServer = player == null;
									arguments.Add(iplayer);
								}

								switch (parameters.Length)
								{
									case 2:
										{
											arguments.Add(cmd);
											break;
										}

									case 3:
										{
											arguments.Add(cmd);
											arguments.Add(args);
											break;
										}
								}
							}
							else
							{
								var primaryParameter = parameters[0].ParameterType;

								arguments.Add(primaryParameter == typeof(BasePlayer) ? player : arg);

								if (primaryParameter == typeof(BasePlayer))
								{
									switch (parameters.Length)
									{
										case 2:
											{
												arguments.Add(cmd);
												break;
											}

										case 3:
											{
												arguments.Add(cmd);
												arguments.Add(args);
												break;
											}
									}
								}
								else
								{
									for (int i = 1; i < parameters.Length; i++)
									{
										arguments.Add(null);
									}
								}
							}
						}

						result = arguments.ToArray();

						if (Interface.CallHook("OnCarbonCommand", arg) == null)
						{
							methodInfo?.Invoke(plugin, result);

							if (!string.IsNullOrEmpty(arg.Reply))
							{
								if (player != null) player.ConsoleMessage(arg.Reply); else Logger.Log(arg.Reply);
							}
						}
					}
					catch (Exception ex)
					{
						if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex);
						else if (plugin is BaseHookable hookable)
							Logger.Error($"[{hookable.Name}] Error", ex.InnerException ?? ex);
					}
				}
				catch (TargetParameterCountException) { }
				catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }

				Pool.FreeList(ref arguments);
				if (result != null) Pool.Free(ref result);
			}, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, Func<Arg, bool> callback, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			AddConsoleCommand(command, plugin, (player, cmd, args) =>
			{
				var arguments = Pool.GetList<object>();
				var result = (object[])null;

				try
				{
					var fullString = args == null || args.Length == 0 ? string.Empty : string.Join(" ", args);
					var client = player == null ? Option.Unrestricted : Option.Client;
					var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
					if (player != null) client = client.FromConnection(player.net.connection);
					client.FromRcon = FromRcon;
					arg.Option = client;
					arg.FullString = fullString;
					arg.Args = args;

					arguments.Add(arg);
					result = arguments.ToArray();

					if (Interface.CallHook("OnCarbonCommand", arg) == null)
					{
						callback.Invoke(arg);

						if (!string.IsNullOrEmpty(arg.Reply))
						{
							if (player != null) player.ConsoleMessage(arg.Reply); else Logger.Log(arg.Reply);
						}
					}
				}
				catch (TargetParameterCountException) { }
				catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }

				Pool.FreeList(ref arguments);
				if (result != null) Pool.Free(ref result);
			}, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddCovalenceCommand(string command, BaseHookable plugin, string method, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true)
		{
			AddChatCommand(command, plugin, method, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
			AddConsoleCommand(command, plugin, method, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddCovalenceCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, bool skipOriginal = true, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true)
		{
			AddChatCommand(command, plugin, callback, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
			AddConsoleCommand(command, plugin, callback, skipOriginal, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}

		public void RemoveChatCommand(string command, BaseHookable plugin = null)
		{
			Community.Runtime.AllChatCommands.RemoveAll(x => x.Command == command && (plugin == null || x.Plugin == plugin));
		}
		public void RemoveConsoleCommand(string command, BaseHookable plugin = null)
		{
			Community.Runtime.AllConsoleCommands.RemoveAll(x => x.Command == command && (plugin == null || x.Plugin == plugin));
		}
	}
}
