using API.Commands;
using ConVar;
using Command = API.Commands.Command;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	private static Dictionary<int, ArgPool> _argumentBuffer = [with(ArgPool.DefaultCapacity)];

	public static string[] AllocateBuffer(int count)
	{
		if (_argumentBuffer.TryGetValue(count, out var pool))
		{
			return pool.Rent();
		}

		_argumentBuffer[count] = pool = new ArgPool(count);
		return pool.Rent();
	}

	public static void ReturnBuffer(string[] buffer)
	{
		if (buffer == null) return;

		if (_argumentBuffer.TryGetValue(buffer.Length, out var pool))
		{
			pool.Return(buffer);
		}
	}

	public class ArgPool
	{
		public static readonly int DefaultCapacity = 10;

		private readonly int length;
		private readonly Stack<string[]> pool;
		private readonly object syncRoot = new();

		private int rentedExtra;
		private int rented;
		private int returned;

		public int RentedExtra => rentedExtra;
		public int Rented => rented;
		public int Returned => returned;
		public int Length => length;
		public int Count => pool.Count;

		public ArgPool(int length)
		{
			this.length = length;
			this.rented = 0;
			this.returned = 0;
			this.rentedExtra = 0;
			pool = new Stack<string[]>(DefaultCapacity);

			for (int i = 0; i < DefaultCapacity; i++)
			{
				this.pool.Push(new string[length]);
			}
		}

		public string[] Rent()
		{
			lock (syncRoot)
			{
				if (pool.Count > 0)
				{
					rented++;
					return pool.Pop();
				}
				else
				{
					rentedExtra++;
					return new string[length];
				}
			}
		}
		public void Return(string[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default;
			}

			lock (syncRoot)
			{
				returned++;
				pool.Push(array);
			}
		}
	}

	public static object IOnPlayerCommand(BasePlayer player, string message, Command.Prefix prefix)
	{
		if (Community.Runtime == null) return Cache.True;

		try
		{
			if (!ConsoleArgEx.TryParseCommand(message.AsSpan()[prefix.Value.Length..], out var command, out var args))
			{
				return Cache.False;
			}

			var stringArgs = AllocateBuffer(args.Length);
			for(int i = 0; i < args.Length; i++)
			{
				stringArgs[i] = args[i].ToString();
			}

			// OnUserCommand
			if (HookCaller.CallStaticHook(2198880635, player, command, stringArgs) != null)
			{
				ReturnBuffer(stringArgs);
				return Cache.False;
			}

			// OnUserCommand
			if (HookCaller.CallStaticHook(2198880635, player.AsIPlayer(), command, stringArgs) != null)
			{
				ReturnBuffer(stringArgs);
				return Cache.False;
			}

			// OnPlayerCommand
			if (HookCaller.CallStaticHook(2915735597, player, command, stringArgs) != null)
			{
				ReturnBuffer(stringArgs);
				return Cache.False;
			}

			ReturnBuffer(stringArgs);

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
				var textMessage = string.Format(phrase, message, result.Select(x => $"{prefix.Value}{x.Result}").ToString(sep1, sep2));

#if !MINIMAL
				player.SendConsoleCommand("chat.add", 2, Community.Runtime.Core.DefaultServerChatId, textMessage);
#else
				player.ChatMessage(textMessage);
#endif
			}
			else
			{
				var core = Community.Runtime.Core;
				var phrase = core.lang.GetMessage("unknown_chat_cmd_1", core, player.UserIDString);
				var textMessage = string.Format(phrase, message);

#if !MINIMAL
				player.SendConsoleCommand("chat.add", 2, Community.Runtime.Core.DefaultServerChatId, textMessage);
#else
				player.ChatMessage(textMessage);
#endif
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
	public static object IOnPlayerChat(ulong playerId, string playerName, ref string message, Chat.ChatChannel channel, BasePlayer basePlayer)
	{
		if (string.IsNullOrEmpty(message) || message.Equals("text"))
		{
			return Cache.True;
		}

		message = message.EscapeRichText();

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
