using API.Commands;
using ConVar;
using Command = API.Commands.Command;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	public static object IOnPlayerCommand(BasePlayer player, string message, Command.Prefix prefix)
	{
		if (Community.Runtime == null) return Cache.True;

		try
		{
			var fullString = message[1..];

			if (string.IsNullOrEmpty(fullString))
			{
				return Cache.False;
			}

			using var split = TempArray<string>.New(fullString.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries));
			var command = split.Get(0).Trim();
			var args = split.Length > 1 ? Facepunch.Extend.StringExtensions.SplitQuotesStrings(fullString[(command.Length + 1)..]) : _emptyStringArray;

			// OnUserCommand
			if (HookCaller.CallStaticHook(2198880635, player, command, args) != null)
			{
				return Cache.False;
			}

			// OnUserCommand
			if (HookCaller.CallStaticHook(2198880635, player.AsIPlayer(), command, args) != null)
			{
				return Cache.False;
			}

			// OnPlayerCommand
			if (HookCaller.CallStaticHook(2915735597, player, command, args) != null)
			{
				return Cache.False;
			}

			if (Community.Runtime.CommandManager.Contains(Community.Runtime.CommandManager.Chat, command, out var cmd))
			{
				var commandArgs = Facepunch.Pool.Get<PlayerArgs>();
				commandArgs.Type = cmd.Type;
				commandArgs.Arguments = args;
				commandArgs.Player = player;
				commandArgs.PrintOutput = true;

				Community.Runtime.CommandManager.Execute(cmd, commandArgs);

				Facepunch.Pool.Free(ref commandArgs);
				return Cache.False;
			}

			if (player.Connection.authLevel >= prefix.SuggestionAuthLevel && Suggestions.Lookup(command, Community.Runtime.CommandManager.Chat.Select(x => x.Name), minimumConfidence: 5) is var result && result.Any())
			{
				var core = Community.Runtime.Core;
				var phrase = core.lang.GetMessage("unknown_chat_cmd_2", core, player.UserIDString);
				var sep1 = core.lang.GetMessage("unknown_chat_cmd_separator_1", core, player.UserIDString);
				var sep2 = core.lang.GetMessage("unknown_chat_cmd_separator_2", core, player.UserIDString);

				player.ChatMessage(string.Format(phrase, message, result.Select(x => $"{prefix.Value}{x.Result}").ToString(sep1, sep2)));
			}
			else
			{
				var core = Community.Runtime.Core;
				var phrase = core.lang.GetMessage("unknown_chat_cmd_1", core, player.UserIDString);
				player.ChatMessage(string.Format(phrase, message));
			}
		}
		catch (Exception ex) { Logger.Error($"Failed IOnPlayerCommand.", ex); }

		return Cache.False;
	}
	internal static object IOnServerCommand(ConsoleSystem.Arg arg)
	{
		if (arg != null && arg.cmd != null && arg.Player() != null && arg.cmd.FullName == "chat.say") return null;

		// OnServerCommand
		return HookCaller.CallStaticHook(2535152661, arg) != null ? Cache.True : null;
	}
	public static object IOnPlayerChat(ulong playerId, string playerName, string message, Chat.ChatChannel channel, BasePlayer basePlayer)
	{
		if (string.IsNullOrEmpty(message) || message.Equals("text"))
		{
			return Cache.True;
		}
		if (basePlayer == null || !basePlayer.IsConnected)
		{
			// OnPlayerOfflineChat
			return HookCaller.CallStaticHook(4068177051, playerId, playerName, message, channel);
		}

		// OnPlayerChat
		var hook1 = HookCaller.CallStaticHook(2032160890, basePlayer, message, channel);

		// OnUserChat
		var hook2 = HookCaller.CallStaticHook(2894159933, basePlayer.AsIPlayer(), message);

		return hook1 ?? hook2;
	}

	internal static object IOnRconInitialize()
	{
		return !Community.Runtime.Config.Rcon ? Cache.False : null;
	}
	internal static object IOnRunCommandLine()
	{
		foreach (var @switch in Facepunch.CommandLine.GetSwitches())
		{
			var value = @switch.Value;

			if (value == "")
			{
				value = "1";
			}

			var key = @switch.Key[1..];
			var options = ConsoleSystem.Option.Unrestricted;
			options.PrintOutput = false;

			ConsoleSystem.Run(options, key, value);
		}

		return Cache.False;
	}
}
