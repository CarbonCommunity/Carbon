using System;
using System.Collections.Generic;
using System.Linq;
using API.Abstracts;
using API.Commands;
using Facepunch;
using UnityEngine;
using Utility;
using Logger = Carbon.Logger;

namespace Components;

public sealed class CommandManager : CarbonBehaviour, ICommandManager
{
	public List<Command> Chat { get; set; } = new();
	public List<Command> ClientConsole { get; set; } = new();
	public List<Command> RCon { get; set; } = new();

	public bool Contains(IList<Command> factory, string command, out Command outCommand)
	{
		var list = Pool.Get<List<Command>>();
		list.AddRange(factory);

		command = command?.Trim().ToLower();

		foreach (var cmd in list)
		{
			if (cmd.Name == command)
			{
				Pool.FreeUnmanaged(ref list);
				outCommand = cmd;
				return true;
			}
		}

		Pool.FreeUnmanaged(ref list);
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
			var list = Pool.Get<List<Command>>();
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

			Pool.FreeUnmanaged(ref list);
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
			Logger.Error($"Failed command execution authentication for command '{command}': {ex}");
			return false;
		}

		try
		{
			command.Callback?.Invoke(args);

			if (!args.PrintOutput && args.IsRCon && !string.IsNullOrEmpty(args.Reply))
			{
				if (args.Tokenize(out ConsoleSystem.Arg argTok))
				{
					if (argTok.Option.RconConnectionId != 0)
					{
						Facepunch.RCon.OnMessage(args.Reply, string.Empty, LogType.Log);
					}
				}
			}

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
							Logger.Log(args.Reply);
						}
						break;

					default:
						Logger.Log(args.Reply);
						break;
				}
			}

			var arg = (ConsoleSystem.Arg)null;

			if (args.PrintOutput)
			{
				if (args.Tokenize(out arg))
				{
					Print(arg.Reply, arg.Player());
				}
				else
				{
					var player = (BasePlayer)null;
					var playerArgs2 = args as PlayerArgs;
					playerArgs2?.GetPlayer(out player);

					Print(args.Reply, player);
				}
			}

			void Print(string reply, BasePlayer player)
			{
				if (string.IsNullOrEmpty(reply))
				{
					return;
				}

				if (player != null)
				{
					player.ConsoleMessage(reply);
				}
				else if(arg != null && arg.IsRcon)
				{
					Facepunch.RCon.OnMessage(reply, string.Empty, UnityEngine.LogType.Log);
				}
				else
				{
					Logger.Log(reply);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			var rconTakeover = false;
			if (!args.PrintOutput && args.IsRCon)
			{
				if (args.Tokenize(out ConsoleSystem.Arg argTok))
				{
					if (argTok.Option.RconConnectionId != 0)
					{
						Facepunch.RCon.OnMessage($"Failed executing command '{command}': {ex}", string.Empty, LogType.Log);
						rconTakeover = true;
					}
				}
			}
			if (!rconTakeover)
			{
				Logger.Error($"Failed executing command '{command}': {ex}");
			}
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
