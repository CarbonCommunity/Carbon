using API.Commands;

namespace Carbon.Components;

/// <summary>
/// Carbon-specific command handling of command line commands.
/// </summary>
public static class CommandLine
{
	internal static readonly string[] _emptyArgs = new string[0];
	internal static readonly char[] _delimiter = new char[] { '|' };

	public static void ExecuteCommands(string @switch, string context, string[] lines = null)
	{
		var arg = CommandLineEx.GetArgumentResult(lines ?? Environment.GetCommandLineArgs(), @switch, string.Empty);
		var split = arg.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);

		if (split.Length > 0)
		{
			Logger.Log($" Executing {split.Length:n0} {split.Length.Plural("command", "commands")} for the '{@switch}' switch ({context}):");
		}

		ExecuteCommands(split);
	}

	public static void ExecuteCommands(string[] commands)
	{
		foreach (var command in commands)
		{
			if (string.IsNullOrEmpty(command))
			{
				continue;
			}

			var split = command.Split(' ');
			var name = split.Length > 0 ? split[0] : string.Empty;

			if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.RCon, name, out var cmd))
			{
				try
				{
					var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
					commandArgs.Type = cmd.Type;
					commandArgs.Arguments = split.Skip(1).ToArray();
					commandArgs.PrintOutput = true;
					commandArgs.IsServer = true;
					commandArgs.IsRCon = true;

					Community.Runtime.CommandManager.Execute(cmd, commandArgs);

					Facepunch.Pool.Free(ref commandArgs);
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed executing Carbon command '{command}'", ex);
				}
			}
			else
			{
				try
				{
					if (ConsoleSystem.Index.All.Any(x => x.FullName == name.Replace("+", string.Empty).Replace("-", string.Empty)))
					{
						ConsoleSystem.Run(ConsoleSystem.Option.Server, command);
					}
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed executing native command '{command}'", ex);
				}
			}
		}
	}
}
