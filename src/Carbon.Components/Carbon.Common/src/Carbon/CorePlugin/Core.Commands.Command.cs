using API.Commands;
using Facepunch;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("find", "Searches through Carbon-processed console commands.")]
	[AuthLevel(2)]
	private void Find(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("command", "value", "help");
		var filter = arg.Args != null && arg.Args.Length > 0 ? arg.GetString(0) : null;

		foreach (var command in Community.Runtime.CommandManager.ClientConsole)
		{
			if (command.HasFlag(CommandFlags.Hidden) || (!string.IsNullOrEmpty(filter) && !command.Name.Contains(filter))) continue;

			var value = " ";
			var moddedStatus = string.Empty;

			if (command.Token != null)
			{
				if (command.Token is FieldInfo field) value = field.GetValue(command.Reference)?.ToString();
				else if (command.Token is PropertyInfo property) value = property.GetValue(command.Reference)?.ToString();
			}

			if (command.HasFlag(CommandFlags.Protected))
			{
				value = new string('*', value.Length);
			}

			if (command.Token != null)
			{
				switch (command.Token)
				{
					case FieldInfo field when field.GetCustomAttribute<CarbonAutoVar>() is CarbonAutoVar autoVar && autoVar.ForceModded:
					case PropertyInfo property when property.GetCustomAttribute<CarbonAutoVar>() is CarbonAutoVar autoVar2 && autoVar2.ForceModded:
						moddedStatus += $" Marks the server to be modded.";
						break;
				}
			}

			body.AddRow($" {command.Name}", value, command.Help + moddedStatus);
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}

	[ConsoleCommand("findchat", "Searches through Carbon-processed chat commands.")]
	[AuthLevel(2)]
	private void FindChat(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("command", "help");
		var filter = arg.Args != null && arg.Args.Length > 0 ? arg.GetString(0) : null;

		foreach (var command in Community.Runtime.CommandManager.Chat)
		{
			if (command.HasFlag(CommandFlags.Hidden) || (!string.IsNullOrEmpty(filter) && !command.Name.Contains(filter))) continue;

			body.AddRow($" {command.Name}", command.Help);
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}

	[ConsoleCommand("moddedvars", "Prints a table/list of all modified Rust ConVars.")]
	[AuthLevel(2)]
	private void ModdedRustConVars(ConsoleSystem.Arg arg)
	{
		using var body = new StringTable("variable", "value", "original_value");
		var filter = arg.Args != null && arg.Args.Length > 0 ? arg.GetString(0) : null;

		foreach (var command in ConVarSnapshots.Snapshots)
		{
			if ((!string.IsNullOrEmpty(filter) && !command.Key.Contains(filter)))
			{
				continue;
			}

			var currentValue = command.Value.Field.Key.GetValue(null)?.ToString();
			var originalValue = command.Value.Value?.ToString();
			if (originalValue != currentValue && !string.IsNullOrEmpty(originalValue) && !string.IsNullOrEmpty(currentValue))
			{
				body.AddRow($" {command.Key}", currentValue, originalValue);
			}
		}

		arg.ReplyWith(body.Write(StringTable.FormatTypes.None));
	}
}
