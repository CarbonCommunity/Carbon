using System;
using System.Collections.Generic;
using System.Linq;
using API.Abstracts;
using API.Commands;
using Facepunch;
using UnityEngine;
using Utility;
using Logger = Utility.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

public sealed class CommandManager : CarbonBehaviour, ICommandManager
{
	public List<Command> Chat { get; set; } = new();
	public List<Command> ClientConsole { get; set; } = new();
	public List<Command> RCon { get; set; } = new();

	public bool Contains(IList<Command> factory, string command, out Command outCommand)
	{
		var list = Pool.GetList<Command>();
		list.AddRange(factory);

		command = command?.Trim().ToLower();

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

	public List<T> GetFactory<T>() where T : Command
	{
		if (typeof(T) == typeof(Command.RCon)) return RCon as List<T>;
		else if (typeof(T) == typeof(Command.ClientConsole)) return ClientConsole as List<T>;
		else if (typeof(T) == typeof(Command.Chat)) return Chat as List<T>;

		return default;
	}

	public List<Command> GetFactory(Command command)
	{
		switch (command)
		{
			case Command.RCon: return RCon;
			case Command.ClientConsole: return ClientConsole;
			case Command.Chat: return Chat;
			default:
				break;
		}

		return default;
	}

	public IEnumerable<T> GetCommands<T>() where T : Command
	{
		if (typeof(T) == typeof(Command.RCon)) return RCon.Cast<T>();
		else if (typeof(T) == typeof(Command.ClientConsole)) return ClientConsole.Cast<T>();
		else if (typeof(T) == typeof(Command.Chat)) return Chat.Cast<T>();

		return default;
	}

	public Command Find(string command)
	{
		if (Contains(Chat, command, out var cmd))
		{
			return cmd;
		}

		if (Contains(ClientConsole, command, out cmd))
		{
			return cmd;
		}

		if (Contains(RCon, command, out cmd))
		{
			return cmd;
		}

		return null;
	}

	public void ClearCommands(Func<Command, bool> condition)
	{
		if (condition == null)
		{
			RCon.Clear();
			ClientConsole.Clear();
			Chat.Clear();
		}
		else
		{
			var list = Pool.GetList<Command>();
			list.AddRange(RCon);
			list.AddRange(ClientConsole);
			list.AddRange(Chat);

			foreach (var command in list)
			{
				if (condition(command))
				{
					if (RCon.Contains(command)) RCon.Remove(command);
					else if (ClientConsole.Contains(command)) ClientConsole.Remove(command);
					else if (Chat.Contains(command)) Chat.Remove(command);
				}
			}

			Pool.FreeList(ref list);
		}
	}

	public bool Execute(Command command, Command.Args args)
	{
		if (command == null) return false;

		try
		{
			if (command.CanExecute != null && !command.CanExecute(command, args))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError($"Failed command execution authentication for command '{command}': {ex}");
			return false;
		}

		try
		{
			command.Callback?.Invoke(args);

			if (args.PrintOutput && !string.IsNullOrEmpty(args.Reply))
			{
				switch (args)
				{
					case PlayerArgs playerArgs:
						if (playerArgs.GetPlayer<BasePlayer>(out var player))
						{
							player.ConsoleMessage(args.Reply);
						}
						else
						{
							Debug.Log(args.Reply);
						}
						break;

					default:
						Debug.Log(args.Reply);
						break;
				}
			}

			if (args.Tokenize<ConsoleSystem.Arg>(out var arg) && arg.Option.PrintOutput)
			{
				Print(arg.Reply, arg.Player());
			}
			else if(args.PrintOutput)
			{
				var player = (BasePlayer)null;
				var playerArgs2 = args as PlayerArgs;
				playerArgs2?.GetPlayer(out player);

				Print(args.Reply, player);
			}

			void Print(string reply, BasePlayer player)
			{
				if (string.IsNullOrEmpty(reply)) return;

				if (player != null)
				{
					player.ConsoleMessage(reply);
				} 
				else if(arg.IsRcon)
				{
					Facepunch.RCon.OnMessage(reply, string.Empty, UnityEngine.LogType.Log);
				}
				else
				{
					Debug.Log(reply);
				}
			}

			args.Dispose();
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError($"Failed executing command '{command}': {ex}");
			return false;
		}
	}

	public bool RegisterCommand(Command command, out string reason)
	{
		if (command == null || string.IsNullOrEmpty(command.Name))
		{
			reason = "Command is null.";
			return false;
		}

		command.Fetch();

		var factory = GetFactory(command);

		if (Contains(factory, command.Name, out _))
		{
			reason = $"Command '{command.Name}' already exists.";
			return false;
		}

		factory.Add(command);

		reason = "Successfully added command.";
		return true;
	}

	public bool UnregisterCommand(Command command, out string reason)
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

		command.Dispose();
		factory.Remove(command);

		reason = "Successfully removed command.";
		return true;
	}
}
