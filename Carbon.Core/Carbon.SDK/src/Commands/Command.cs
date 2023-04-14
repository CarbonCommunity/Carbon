using System;
using static Carbon.Base.Command;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

[Flags]
public enum CommandFlags
{
	None,
	Hidden,
	Protected
}

public class Command
{
	public static bool FromRcon { get; set; }

	public string Name { get; set; }
	public string Help { get; set; }
	public object Token { get; set; }
	public object Reference { get; set; }
	public CommandFlags Flags { get; set; } = CommandFlags.None;
	public Action<Args> Callback { get; set; }
	public Func<Command, Args, bool> CanExecute { get; set; } 

	public class Args
	{
		public string[] Arguments { get; set; }
	}

	public class RCon : Command
	{

	}
	public class Chat : AuthenticatedCommand
	{

	}
	public class Console : AuthenticatedCommand
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

public class PlayerArgs : Args
{
	public object Player { get; set; }
}
