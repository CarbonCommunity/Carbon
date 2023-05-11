using System;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Commands;

public class Command
{
	public enum Types
	{
		Generic,
		Chat,
		Console,
		Rcon
	}

	public static bool FromRcon { get; set; }

	public string Name { get; set; }
	public string Help { get; set; }
	public object Token { get; set; }
	public object Reference { get; set; }
	public Types Type { get; set; }
	public CommandFlags Flags { get; set; } = CommandFlags.None;
	public Action<Args> Callback { get; set; }
	public Func<Command, Args, bool> CanExecute { get; set; }

	public void Fetch()
	{
		Name = Name?.ToLower().Trim();
		Help = Help?.Trim();

		switch (this)
		{
			case RCon:
				Type = Types.Rcon;
				break;
			case ClientConsole:
				Type = Types.Console;
				break;
			case Chat:
				Type = Types.Chat;
				break;
			default:
				break;
		}
	}

	public class Args
	{
		public Types Type { get; set; }
		public string[] Arguments { get; set; }

		public string Reply { get; set; }
		public object Token { get; set; }

		public void ReplyWith(string message)
		{
			Reply = message;
		}
		public void ReplyWith<T>(T message)
		{
			Reply = JsonConvert.SerializeObject(message, Formatting.Indented);
		}
	}

	public class RCon : Command
	{

	}
	public class Chat : AuthenticatedCommand
	{

	}
	public class ClientConsole : AuthenticatedCommand
	{

	}

	public class Authentication
	{
		public int AuthLevel { get; set; }
		public string[] Permissions { get; set; }
		public string[] Groups { get; set; }
		public int Cooldown { get; set; }
	}

	public bool HasFlag(CommandFlags flag)
	{
		return (Flags & flag) != 0;
	}
	public void SetFlag(CommandFlags flag, bool wants)
	{
		switch (wants)
		{
			case true:
				Flags |= flag;
				break;

			case false:
				Flags &= ~flag;
				break;
		}
	}
	public void ClearFlags()
	{
		Flags = CommandFlags.None;
	}
}

public class AuthenticatedCommand : Command
{
	public Authentication Auth { get; set; }
}

public class PlayerArgs : Command.Args
{
	public object Player { get; set; }

	public bool GetPlayer<T>(out T value) where T : class
	{
		return (value = (T)Player) != null;
	}
}
