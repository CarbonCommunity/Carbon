using API.Commands;
using static ConsoleSystem;
using Logger = Carbon.Logger;
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
		public static bool FromRcon
		{
			get; set;
		}

		internal readonly Func<API.Commands.Command, API.Commands.Command.Args, bool> _playerExecute = (cmd, args) =>
		{
			if (args is PlayerArgs playerArgs && playerArgs != null)
			{
				var player = playerArgs.Player as BasePlayer;
				var authenticatedCommand = cmd as AuthenticatedCommand;

				if (player != null && authenticatedCommand != null)
				{
					if (authenticatedCommand.Auth.Permissions != null)
					{
						var hasPerm = authenticatedCommand.Auth.Permissions.Count(x => !string.IsNullOrEmpty(x)) == 0;
						foreach (var permission in authenticatedCommand.Auth.Permissions)
						{
							if (Community.Runtime.CorePlugin.permission.UserHasPermission(player.UserIDString, permission))
							{
								hasPerm = true;
								break;
							}
						}

						if (!hasPerm)
						{
							player?.ConsoleMessage($"You don't have any of the required permissions to run this command.");
							return false;
						}
					}

					if (authenticatedCommand.Auth.Groups != null)
					{
						var hasGroup = authenticatedCommand.Auth.Groups.Count(x => !string.IsNullOrEmpty(x)) == 0;
						foreach (var group in authenticatedCommand.Auth.Groups)
						{
							if (Community.Runtime.CorePlugin.permission.UserHasGroup(player.UserIDString, group))
							{
								hasGroup = true;
								break;
							}
						}

						if (!hasGroup)
						{
							player?.ConsoleMessage($"You aren't in any of the required groups to run this command.");
							return false;
						}
					}

					if (authenticatedCommand.Auth.AuthLevel != -1)
					{
						var hasAuth = player.Connection.authLevel >= authenticatedCommand.Auth.AuthLevel;

						if (!hasAuth)
						{
							player?.ConsoleMessage($"You don't have the minimum auth level [{authenticatedCommand.Auth.AuthLevel}] required to execute this command [your level: {player.Connection.authLevel}].");
							return false;
						}
					}

					if (CarbonPlugin.IsCommandCooledDown(player, cmd.Name, authenticatedCommand.Auth.Cooldown, out var timeLeft, true))
					{
						player.ChatMessage($"You're cooled down. Please wait {TimeEx.Format(timeLeft).ToLower()}.");
						return false;
					}
				}
			}

			return true;
		};

		public void AddChatCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			var cmd = new API.Commands.Command.Chat
			{
				Name = command,
				Reference = plugin,
				Callback = arg =>
				{
					switch (arg)
					{
						case PlayerArgs playerArgs:
							try { callback?.Invoke(playerArgs.Player as BasePlayer, command, arg.Arguments); }
							catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }
							break;
					}
				},
				Help = help,
				Token = reference,
				Auth = new API.Commands.Command.Authentication
				{
					AuthLevel = authLevel,
					Permissions = permissions,
					Groups = groups,
					Cooldown = cooldown,
				},
				CanExecute = _playerExecute
			};
			cmd.SetFlag(CommandFlags.Hidden, isHidden);
			cmd.SetFlag(CommandFlags.Protected, @protected);

			if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
			{
				Logger.Warn(reason);
			}
		}
		public void AddChatCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			AddChatCommand(command, plugin, (player, cmd, args) =>
			{
				var arguments = Pool.GetList<object>();
				var result = (object[])null;
				var arg = (ConsoleSystem.Arg)null;

				try
				{
					var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters ().Any () || x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer))));
					var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters ().Any () || x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer))));
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
						else
						{
							if (parameters[0].ParameterType == typeof(Arg))
							{
								var fullString = args == null || args.Length == 0 ? string.Empty : string.Join(" ", args);
								var client = player == null ? Option.Unrestricted : Option.Client;
								arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
								if (player != null) client = client.FromConnection(player.net.connection);
								client.FromRcon = FromRcon;
								arg.Option = client;
								arg.FullString = fullString;
								arg.Args = args;

								arguments.Add(arg);
							}
							else
							{
								arguments.Add(player);
							}
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

						var currentArgumentCount = arguments.Count;

						if (parameters.Length > currentArgumentCount)
						{
							for (int i = 0; i < parameters.Length - currentArgumentCount; i++)
							{
								arguments.Add(null);
							}
						}

						result = arguments.ToArray();
					}

					methodInfo?.Invoke(plugin, result);
				}
				catch (Exception ex) { if (plugin is RustPlugin rustPlugin) rustPlugin.LogError("Error", ex.InnerException ?? ex); }

				if (arguments != null) Pool.FreeList(ref arguments);
				if (result != null) Array.Clear(result, 0, result.Length);
				if (arg != null) arg = null;
			}, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
		{
			// Client console
			{
				var cmd = new API.Commands.Command.ClientConsole
				{
					Name = command,
					Reference = plugin,
					Callback = arg =>
					{
						switch (arg)
						{
							case PlayerArgs playerArgs:
								callback?.Invoke(playerArgs.Player as BasePlayer, command, arg.Arguments);
								break;
						}
					},
					Help = help,
					Token = reference,
					Auth = new API.Commands.Command.Authentication
					{
						AuthLevel = authLevel,
						Permissions = permissions,
						Groups = groups,
						Cooldown = cooldown,
					},
					CanExecute = _playerExecute,
				};
				cmd.SetFlag(CommandFlags.Hidden, isHidden);
				cmd.SetFlag(CommandFlags.Protected, @protected);

				if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
				{
					Logger.Warn(reason);
				}
			}

			// RCon console
			{
				var cmd = new API.Commands.Command.RCon
				{
					Name = command,
					Reference = plugin,
					Callback = arg =>
					{
						callback?.Invoke(null, command, arg.Arguments);
					},
					Help = help,
					Token = reference,
					CanExecute = _playerExecute,
				};
				cmd.SetFlag(CommandFlags.Hidden, isHidden);
				cmd.SetFlag(CommandFlags.Protected, @protected);

				if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
				{
					Logger.Warn(reason);
				}
			}
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
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
					arg.cmd = Community.Runtime.CommandManager.Find(command)?.RustCommand;

					try
					{
						var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer))));
						var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer))));
						var methodInfo = covalenceMethod ?? consoleMethod;
						var parameters = methodInfo.GetParameters();

						if (parameters.Length > 0)
						{
							if (methodInfo == covalenceMethod)
							{
								if (player == null)
								{
									arguments.Add(new RustPlayer { IsServer = true });
								}
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

						if (Interface.CallHook("OnConsoleCommand", arg) == null)
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
				if (result != null) Array.Clear(result, 0, result.Length);
			}, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddConsoleCommand(string command, BaseHookable plugin, Func<Arg, bool> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false)
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
					arg.cmd = Community.Runtime.CommandManager.Find(command)?.RustCommand;

					arguments.Add(arg);
					result = arguments.ToArray();

					if (Interface.CallHook("OnConsoleCommand", arg) == null)
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
				if (result != null) Array.Clear(result, 0, result.Length);
			}, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddCovalenceCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true)
		{
			AddChatCommand(command, plugin, method, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
			AddConsoleCommand(command, plugin, method, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}
		public void AddCovalenceCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true)
		{
			AddChatCommand(command, plugin, callback, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
			AddConsoleCommand(command, plugin, callback, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent);
		}

		public void RemoveChatCommand(string command, BaseHookable plugin = null)
		{
			Community.Runtime.CommandManager.ClearCommands(cmd => cmd.Name == command && (plugin == null || cmd.Reference == plugin));
		}
		public void RemoveConsoleCommand(string command, BaseHookable plugin = null)
		{
			Community.Runtime.CommandManager.ClearCommands(cmd => cmd.Name == command && (plugin == null || cmd.Reference == plugin));
		}
	}
}
