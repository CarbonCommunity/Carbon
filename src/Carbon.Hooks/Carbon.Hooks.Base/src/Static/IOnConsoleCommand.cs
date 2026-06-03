using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using API.Commands;
using API.Hooks;
using Carbon.Extensions;
using Facepunch;
using Facepunch.Extend;
using static ConsoleSystem;
using Command = API.Commands.Command;

namespace Carbon.Hooks;

#pragma warning disable IDE0051
#pragma warning disable IDE0060

public partial class Category_Static
{
	public partial class Static_ConsoleSystem
	{
		[HookAttribute.Patch("OnConsoleCommand", "OnConsoleCommand", typeof(ConsoleSystem), nameof(RunWithResult), [typeof(Option), typeof(string), typeof(object[])])]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		[MetadataAttribute.Info("Called whenever a Carbon server command is called.")]

		public class IOnConsoleCommand : Patch
		{
			private static string Space = " ";
			private static readonly string[] Filters = ["no_input"];

			public static bool Prefix(ref CommandResult __result, Option options, ref string strCommand, object[] args)
			{
				if (Community.Runtime == null || Filters.Contains(strCommand))
				{
					return true;
				}

				try
				{
					if (!ConsoleArgEx.TryParseCommand(strCommand, out var command, out var parsedArguments))
					{
						__result = new CommandResult(CommandResultType.CommandNotFound, null, null);
						return false;
					}

					if (Command.FromRcon)
					{
						return true;
					}

					var argsLength = args?.Length ?? 0;
					string[] arguments;
					if (argsLength == 0)
					{
						arguments = parsedArguments;
					}
					else
					{
						arguments = new string[parsedArguments.Length + argsLength];
						Array.Copy(parsedArguments, arguments, parsedArguments.Length);
						for (var i = 0; i < argsLength; i++)
						{
							arguments[parsedArguments.Length + i] = args[i]?.ToString();
						}
					}

					var player = options.Connection?.player as BasePlayer;
					var commands = player == null ? Community.Runtime.CommandManager.RCon : Community.Runtime.CommandManager.ClientConsole;

					if (Community.Runtime.Config.Aliases.TryGetValue(command, out var alias))
					{
						command = alias;
						strCommand = argsLength == 0 && parsedArguments.Length == 0
							? alias
							: $"{alias} {arguments.ToString(Space)}";
					}

					if (Community.Runtime.CommandManager.Contains(commands, command, out var commandInstance))
					{
						var arg = new Arg(options, strCommand);
						arg.cmd = commandInstance.RustCommand;
						arg.Invalid = false;

						var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
						commandArgs.Token = arg;
						commandArgs.Type = commandInstance.Type;
						commandArgs.Arguments = arguments;
						commandArgs.Player = player;
						commandArgs.IsServer = player == null;
						commandArgs.PrintOutput = options.PrintOutput || player != null;

						Command.FromRcon = false;
						Community.Runtime.CommandManager.Execute(commandInstance, commandArgs);
						__result = new CommandResult(CommandResultType.Success, arg.Reply, arg.cmd);
						Facepunch.Pool.Free(ref commandArgs);
						return false;
					}

					if (Community.Runtime.Config.Logging.CommandSuggestions)
					{
						if (player != null && !player.IsAdmin)
						{
							return true;
						}

						if (ConsoleSystem.Index.Server.Find(command) != null)
						{
							return true;
						}
					}
				}
				catch (Exception exception)
				{
					Logger.Error($"Failed ConsoleSystem.Run [{strCommand}] [{string.Join(" ", args)}]", exception);
				}

				return true;
			}
		}
	}
}
