using System;
using System.Collections.Generic;
using Carbon.Base;
using Carbon.Contracts;
using Facepunch;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

public class CommandProcessor : BaseProcessor, IDisposable, ICommandProcessor
{
	public override string Name => "Command Processor";

	internal List<BaseCommand> All { get; set; } = new();

	internal bool Contains(string command, out BaseCommand outCommand)
	{
		var list = Pool.GetList<BaseCommand>();
		list.AddRange(All);

		foreach(var cmd in All)
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

	public bool RegisterCommand(BaseCommand command, out string reason)
	{
		if(command== null)
		{
			reason = "Command is null.";
			return false;
		}

		if(Contains(command.Name, out _))
		{
			reason = "Command already exists.";
			return false;
		}

		All.Add(command);

		reason = "Successfully added command.";
		return true;
	}
	public bool UnregisterCommand(string command, out string reason)
	{
		if (string.IsNullOrEmpty(command))
		{
			reason = "Command is null or empty.";
			return false;
		}

		if (!Contains(command, out var outCommand))
		{
			reason = "Command does not exist.";
			return false;
		}

		All.Remove(outCommand);
		reason = "Successfully removed command.";
		return true;
	}
}
