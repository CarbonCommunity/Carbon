using API.Commands;
using Facepunch;
using static ConsoleSystem;
using Logger = Carbon.Logger;
using Pool = Facepunch.Pool;
using Carbon.Extensions;

namespace Oxide.Game.Rust.Libraries;

public class Command : Library
{
	public static bool FromRcon
	{
		get; set;
	}

	private static void LogCommandCompatibilityError(string commandType, string command, BaseHookable plugin, Exception ex, string label, BasePlayer player, bool isChat, bool notifyPlayer = true)
	{
		Logger.Error($"Failed executing {commandType} command '{command}' in '{plugin.ToPrettyString()}' [{label}]: {ex.GetCompatibilityMessage()}", ex.GetCompatibilityException());
		if (notifyPlayer && Community.Runtime.Config.Logging.ShowCommandCompatibilityErrors && player != null)
		{
			if (isChat) player.ChatMessage(Localisation.Get("cmd_failed_compat", player.UserIDString));
			else player.ConsoleMessage(Localisation.Get("cmd_failed_compat", player.UserIDString));
		}
	}

	private static void LogCommandGenericError(string commandType, string command, BaseHookable plugin, Exception ex, string label)
	{
		Logger.Error($"Failed executing {commandType} command '{command}' in '{plugin.ToPrettyString()}' [{label}]", ex is TargetInvocationException invocation ? invocation.InnerException ?? ex : ex);
	}

	private Func<API.Commands.Command, API.Commands.Command.Args, bool> OnPlayerExecute(bool isChat)
	{
		return (cmd, args) =>
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
							if (Community.Runtime.Core.permission.UserHasPermission(player.UserIDString, permission))
							{
								hasPerm = true;
								break;
							}
						}

						if (!hasPerm)
						{
							if (isChat) player.ChatMessage(Localisation.Get("no_perm", player.UserIDString));
							else player.ConsoleMessage(Localisation.Get("no_perm", player.UserIDString));
							return false;
						}
					}

					if (authenticatedCommand.Auth.Groups != null)
					{
						var hasGroup = authenticatedCommand.Auth.Groups.Count(x => !string.IsNullOrEmpty(x)) == 0;
						foreach (var group in authenticatedCommand.Auth.Groups)
						{
							if (Community.Runtime.Core.permission.UserHasGroup(player.UserIDString, group))
							{
								hasGroup = true;
								break;
							}
						}

						if (!hasGroup)
						{
							if (isChat) player.ChatMessage(Localisation.Get("no_group", player.UserIDString));
							else player.ConsoleMessage(Localisation.Get("no_group", player.UserIDString));
							return false;
						}
					}

					if (authenticatedCommand.Auth.AuthLevel != -1)
					{
						var hasAuth = player.Connection.authLevel >= authenticatedCommand.Auth.AuthLevel;

						if (!hasAuth)
						{
							if (isChat) player.ChatMessage(Localisation.Get("no_auth", player.UserIDString, authenticatedCommand.Auth.AuthLevel, player.Connection.authLevel));
							else player.ConsoleMessage(Localisation.Get("no_auth", player.UserIDString, authenticatedCommand.Auth.AuthLevel, player.Connection.authLevel));
							return false;
						}
					}

					if (!(Community.Runtime.Config.Permissions.BypassAdminCooldowns && player.Connection.authLevel > 1) && CarbonPlugin.IsCommandCooledDown(player, cmd.Name, authenticatedCommand.Auth.Cooldown, out var timeLeft, doCooldownPenalty: authenticatedCommand.Auth.DoCooldownPenalty))
					{
						if (timeLeft < 2f) return false;
						if (isChat) player.ChatMessage(Localisation.Get("cooldown_player", player.UserIDString, TimeEx.Format(timeLeft).ToLower()));
						else player.ConsoleMessage(Localisation.Get("cooldown_player", player.UserIDString, TimeEx.Format(timeLeft).ToLower()));
						return false;
					}
				}
			}

			return true;
		};
	}

	public void AddChatCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
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
						try { callback?.Invoke(playerArgs.Player as BasePlayer, command, arg.Arguments.ToStringArray()); }
						catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("chat", command, plugin, ex, "callback", playerArgs.Player as BasePlayer, isChat: true); }
						catch (Exception ex) { LogCommandGenericError("chat", command, plugin, ex, "callback"); }
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
				DoCooldownPenalty = doCooldownPenalty,
			},
			CanExecute = OnPlayerExecute(true)
		};
		cmd.SetFlag(CommandFlags.Hidden, isHidden);
		cmd.SetFlag(CommandFlags.Protected, @protected);

		if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
		{
			Logger.Warn(reason);
		}
	}
	public void AddChatCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
	{
		var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer))));
		var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer))));
		var methodInfo = covalenceMethod ?? consoleMethod;

		AddChatCommand(command, plugin, methodInfo, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
	}
	public void AddChatCommand(string command, BaseHookable plugin, MethodInfo methodInfo, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
	{
		AddChatCommand(command, plugin, (player, cmd, args) =>
		{
			var arguments = Pool.Get<List<object>>();
			var result = (object[])null;
			var arg = (ConsoleSystem.Arg)null;

			try
			{
				var parameters = methodInfo.GetParameters();
				var covalenceMethod = parameters.Length > 0 && parameters.Any(y => y.ParameterType == typeof(IPlayer));

				if (parameters.Length > 0)
				{
					if (covalenceMethod)
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
							arg.Args = [.. (args ?? []).Select(x => (StringView)x)];

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
			catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("chat", command, plugin, ex, "callback", player, isChat: true); }
			catch (Exception ex) { LogCommandGenericError("chat", command, plugin, ex, "callback"); }

			if (arguments != null)
			{
				Pool.FreeUnmanaged(ref arguments);
			}
			if (result != null)
			{
				Array.Clear(result, 0, result.Length);
			}

			arg = null;
		}, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
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
							try { callback?.Invoke(playerArgs.Player as BasePlayer, command, arg.Arguments.ToStringArray()); }
							catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "callback", playerArgs.Player as BasePlayer, isChat: false); }
							catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "callback"); }
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
					DoCooldownPenalty = doCooldownPenalty,
				},
				CanExecute = OnPlayerExecute(false)
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
				Callback = args =>
				{
					try { callback?.Invoke(null, command, args.Arguments.ToStringArray()); }
					catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "callback", null, isChat: false, notifyPlayer: false); if (!args.PrintOutput) args.Reply = ex.Message; }
					catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "callback"); if (!args.PrintOutput) args.Reply = ex.Message; }
				},
				Help = help,
				Token = reference,
				CanExecute = OnPlayerExecute(false)
			};
			cmd.SetFlag(CommandFlags.Hidden, isHidden);
			cmd.SetFlag(CommandFlags.Protected, @protected);

			if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
			{
				Logger.Warn(reason);
			}
		}
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
	{
		var methodInfos = plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		var covalenceMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType == typeof(IPlayer))));
		var consoleMethod = methodInfos.FirstOrDefault(x => x.Name == method && (!x.GetParameters().Any() || x.GetParameters().Any(y => y.ParameterType != typeof(IPlayer))));
		var methodInfo = covalenceMethod ?? consoleMethod;

		AddConsoleCommand(command, plugin, methodInfo, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, MethodInfo methodInfo, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
	{
		AddConsoleCommand(command, plugin, (player, cmd, args) =>
		{
			var arguments = Pool.Get<List<object>>();
			var result = (object[])null;

			try
			{
				var fullString = args == null || args.Length == 0 ? string.Empty : string.Join(" ", args);
				var option = player == null ? Option.Server : Option.Client;
				var arg = FormatterServices.GetUninitializedObject(typeof(Arg)) as Arg;
				if (player != null) option = option.FromConnection(player.net.connection);
				arg.Option = option;
				arg.FullString = fullString;
				arg.Args = (args ?? []).ToStringViewArray();
				arg.cmd = Community.Runtime.CommandManager.Find(command)?.RustCommand;

				try
				{
					var parameters = methodInfo.GetParameters();
					var covalenceMethod = parameters.Length > 0 && parameters.Any(y => y.ParameterType == typeof(IPlayer));

					if (parameters.Length > 0)
					{
						if (covalenceMethod)
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

					// OnConsoleCommand / OnServerCommand
					if (HookCaller.CallStaticHook(39952195, arg) == null && HookCaller.CallStaticHook(2535152661, arg) == null)
					{
						methodInfo?.Invoke(plugin, result);

						if (!string.IsNullOrEmpty(arg.Reply) && option.PrintOutput)
						{
							if (player != null) player.ConsoleMessage(arg.Reply);
							else if (FromRcon) Facepunch.RCon.OnMessage(arg.Reply, string.Empty, UnityEngine.LogType.Log);
							else Logger.Log(arg.Reply);
						}
					}
				}
				catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "callback", player, isChat: false); }
				catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "callback"); }
			}
			catch (TargetParameterCountException) { }
			catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "internal", player, isChat: false); }
			catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "internal"); }

			Pool.FreeUnmanaged(ref arguments);

			if (result != null)
			{
				Array.Clear(result, 0, result.Length);
			}
		}, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
	}
	public void AddConsoleCommand(string command, BaseHookable plugin, Func<Arg, bool> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = false, bool doCooldownPenalty = false)
	{
		// Client console
		{
			var cmd = new API.Commands.Command.ClientConsole
			{
				Name = command,
				Reference = plugin,
				Callback = args =>
				{
					if (!args.Tokenize(out Arg arg))
					{
						return;
					}
					try
					{
						callback?.Invoke(arg);
						args.Reply = arg.Reply;
						args.PrintOutput = arg.Option.PrintOutput;
					}
					catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "callback", arg.Player(), isChat: false); }
					catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "callback"); }
				},
				Help = help,
				Token = reference,
				Auth = new API.Commands.Command.Authentication
				{
					AuthLevel = authLevel,
					Permissions = permissions,
					Groups = groups,
					Cooldown = cooldown,
					DoCooldownPenalty = doCooldownPenalty,
				},
				CanExecute = OnPlayerExecute(false)
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
				Callback = args =>
				{
					if (!args.Tokenize(out Arg arg))
					{
						return;
					}

					args.PrintOutput = arg.Option.PrintOutput;
					try
					{
						callback?.Invoke(arg);
						args.Reply = arg.Reply;
					}
					catch (Exception ex) when (ex.IsCompatibilityError()) { LogCommandCompatibilityError("console", command, plugin, ex, "callback", arg.Player(), isChat: false, notifyPlayer: false); if (!args.PrintOutput) args.Reply = ex.Message; }
					catch (Exception ex) { LogCommandGenericError("console", command, plugin, ex, "callback"); if (!args.PrintOutput) args.Reply = ex.Message; }
				},
				Help = help,
				Token = reference,
				CanExecute = OnPlayerExecute(false)
			};
			cmd.SetFlag(CommandFlags.Hidden, isHidden);
			cmd.SetFlag(CommandFlags.Protected, @protected);

			if (!Community.Runtime.CommandManager.RegisterCommand(cmd, out var reason) && !silent)
			{
				Logger.Warn(reason);
			}
		}
	}
	public void AddCovalenceCommand(string command, BaseHookable plugin, string method, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true, bool doCooldownPenalty = false)
	{
		AddChatCommand(command, plugin, method, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
		AddConsoleCommand(command, plugin, method, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
	}
	public void AddCovalenceCommand(string command, BaseHookable plugin, Action<BasePlayer, string, string[]> callback, string help = null, object reference = null, string[] permissions = null, string[] groups = null, int authLevel = -1, int cooldown = 0, bool isHidden = false, bool @protected = false, bool silent = true, bool doCooldownPenalty = false)
	{
		AddChatCommand(command, plugin, callback, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
		AddConsoleCommand(command, plugin, callback, help, reference, permissions, groups, authLevel, cooldown, isHidden, @protected, silent, doCooldownPenalty);
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
