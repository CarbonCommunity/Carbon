using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Contracts;
using Facepunch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class CommandManager : FacepunchBehaviour, ICommandManager
{
	public List<Base.Command> RCon { get; set; } = new();
	public List<Base.Command> Console { get; set; } = new();
	public List<Base.Command> Chat { get; set; } = new();

	public bool Contains(IList<Base.Command> factory, string command, out Base.Command outCommand)
	{
		var list = Pool.GetList<Base.Command>();
		list.AddRange(factory);

		foreach (var cmd in list)
		{
			if (cmd.Name == command)
			{
				Pool.FreeList(ref list);
				outCommand = cmd;
				return true;
			}
		}

		Pool.FreeList(ref list);
		outCommand = default;
		return false;
	}
	public List<T> GetFactory<T>() where T : Base.Command
	{
		if (typeof(T) == typeof(Base.Command.RCon)) return RCon as List<T>;
		else if (typeof(T) == typeof(Base.Command.Console)) return Console as List<T>;
		else if (typeof(T) == typeof(Base.Command.Chat)) return Chat as List<T>;

		return default;
	}
	public List<Base.Command> GetFactory(Base.Command command)
	{
		switch (command)
		{
			case Base.Command.RCon: return RCon;
			case Base.Command.Console: return Console;
			case Base.Command.Chat: return Chat;
			default:
				break;
		}

		return default;
	}

	public IEnumerable<T> GetCommands<T>() where T : Base.Command
	{
		if (typeof(T) == typeof(Base.Command.RCon)) return RCon.Cast<T>();
		else if (typeof(T) == typeof(Base.Command.Console)) return Console.Cast<T>();
		else if (typeof(T) == typeof(Base.Command.Chat)) return Chat.Cast<T>();

		return default;
	}

	public void ClearCommands(Func<Base.Command, bool> condition)
	{
		if (condition == null)
		{
			RCon.Clear();
			Console.Clear();
			Chat.Clear();
		}
		else
		{
			var list = Pool.GetList<Base.Command>();
			list.AddRange(RCon);
			list.AddRange(Console);
			list.AddRange(Chat);

			foreach (var command in list)
			{
				if (condition(command))
				{
					if (RCon.Contains(command)) RCon.Remove(command);
					else if (Console.Contains(command)) Console.Remove(command);
					else if (Chat.Contains(command)) Chat.Remove(command);
				}
			}

			Pool.FreeList(ref list);
		}
	}
	public bool Execute(Base.Command command, Base.Command.Args args)
	{
		if (command == null) return false;

		try
		{
			if (!command.CanExecute(command, args))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed command execution authentication for command '{command}'", ex);
			return false;
		}

		try
		{
			command.Callback?.Invoke(args);
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed executing command '{command}'", ex);
			return false;
		}
	}

	public bool RegisterCommand(Base.Command command, out string reason)
	{
		if (command == null)
		{
			reason = "Command is null.";
			return false;
		}

		var factory = GetFactory(command);

		if (Contains(factory, command.Name, out _))
		{
			reason = "Command already exists.";
			return false;
		}

		factory.Add(command);

		reason = "Successfully added command.";
		return true;
	}
	public bool UnregisterCommand(Base.Command command, out string reason)
	{
		var factory = GetFactory(command);

		if (factory == null)
		{
			reason = "Couldn't find factory.";
			return false;
		}

		if (!factory.Contains(command))
		{
			reason = "Couldn't find the command.";
			return false;
		}

		factory.Remove(command);

		reason = "Successfully removed command.";
		return true;
	}
}
