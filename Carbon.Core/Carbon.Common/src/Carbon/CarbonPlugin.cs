using System.Collections.Generic;
using System;
using Carbon.Components;
using Oxide.Core;
using Oxide.Plugins;
using Carbon.Extensions;
using System.Linq;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Plugins;

public class CarbonPlugin : RustPlugin
{
	public CUI.Handler CuiHandler { get; set; }

	public override void Setup(string name, string author, VersionNumber version, string description)
	{
		base.Setup(name, author, version, description);

		CuiHandler = new CUI.Handler();
	}

	#region CUI

	public CUI CreateCUI()
	{
		return new CUI(CuiHandler);
	}

	#endregion

	#region Command Cooldown

	internal static Dictionary<BasePlayer, List<CooldownInstance>> _commandCooldownBuffer = new();

	public static bool IsCommandCooledDown(BasePlayer player, string command, int time, bool doCooldownIfNot = true)
	{
		if (time == 0 || player == null) return false;

		if (!_commandCooldownBuffer.TryGetValue(player, out var pairs))
		{
			_commandCooldownBuffer.Add(player, pairs = new List<CooldownInstance>());
		}

		var lookupCommand = pairs.FirstOrDefault(x => x.Command == command);
		if (lookupCommand == null)
		{
			pairs.Add(lookupCommand = new CooldownInstance { Command = command });
		}

		var timePassed = (DateTime.Now - lookupCommand.LastCall);
		if (timePassed.TotalMilliseconds >= time)
		{
			if (doCooldownIfNot)
			{
				lookupCommand.LastCall = DateTime.Now;
			}
			return false;
		}

		var log = $"You're cooled down. Please wait {TimeEx.Format((time - timePassed.TotalMilliseconds) * 0.001).ToLower()}.";
		player.ChatMessage(log);
		player.ConsoleMessage(log);
		return true;
	}

	internal sealed class CooldownInstance
	{
		public string Command;
		public DateTime LastCall;
	}

	#endregion
}
