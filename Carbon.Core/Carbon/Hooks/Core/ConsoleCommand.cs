///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon;
using Carbon.Core;
using Carbon.Core.Extensions;
using Facepunch.Extend;
using Harmony;

[HarmonyPatch(typeof(ConsoleSystem), "Run")]
public class ConsoleCommand
{
	internal static string[] EmptyArgs = new string[0];

	public static bool Prefix(ConsoleSystem.Option options, string strCommand, object[] args)
	{
		if (CarbonCore.Instance == null) return true;

		try
		{
			var split = strCommand.Split(ConsoleArgEx.CommandSpacing, StringSplitOptions.RemoveEmptyEntries);
			var command = split[0].Trim();
			var args2 = split.Length > 1 ? strCommand.Substring(command.Length + 1).SplitQuotesStrings() : EmptyArgs;
			Facepunch.Pool.Free(ref split);

			foreach (var cmd in CarbonCore.Instance.AllConsoleCommands)
			{
				if (cmd.Command == command)
				{
					try
					{
						cmd.Callback?.Invoke(options.Connection?.player as BasePlayer, command, args2);
					}
					catch (Exception ex)
					{
						Carbon.Logger.Error("ConsoleSystem_Run", ex);
					}

					return false;
				}
			}
		}
		catch { }

		return true;
	}
}
