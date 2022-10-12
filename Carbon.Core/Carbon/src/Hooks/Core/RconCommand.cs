///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Linq;
using System.Net;
using Carbon.Core;
using Carbon.Core.Extensions;
using Facepunch;
using Oxide.Core;

[Hook.AlwaysPatched]
[Hook("OnRconCommand"), Hook.Category(Hook.Category.Enum.Server)]
[Hook.Parameter("address", typeof(IPAddress))]
[Hook.Parameter("command", typeof(string))]
[Hook.Parameter("args", typeof(string[]))]
[Hook.Info("Called when an RCON command is run.")]
[Hook.Patch(typeof(RCon), "OnCommand")]
public class RconCommand
{
	public static bool Prefix(RCon.Command cmd)
	{
		if (CarbonCore.Instance == null) return true;

		try
		{
			var split = cmd.Message.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
			var command = split[0].Trim();

			var arguments = Pool.GetList<string>();
			foreach (var arg in split.Skip(1)) arguments.Add(arg.Trim());
			var args2 = arguments.ToArray();
			Pool.FreeList(ref arguments);

			if (Interface.CallHook("OnRconCommand", cmd.Ip, command, args2) != null)
			{
				return false;
			}

			foreach (var carbonCommand in CarbonCore.Instance.AllConsoleCommands)
			{
				if (carbonCommand.Command == command)
				{
					try
					{
						carbonCommand.Callback?.Invoke(null, command, args2);
						return !carbonCommand.SkipOriginal;
					}
					catch (Exception ex)
					{
						Carbon.Logger.Error("RconCommand_OnCommand", ex);
					}

					break;
				}
			}
		}
		catch { }

		return true;
	}
}
