using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Plugins;
using static Carbon.Base.BaseCommand;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Base;

public class BaseCommand
{
	public string Name { get; set; }
	public Action<Args> Callback { get; set; }

	public virtual bool CanExecute(Args arg)
	{
		return true;
	}

	public class Args
	{
		public string Command { get; set; }
		public string[] Arguments { get; set; }
	}

	public class Rcon : BaseCommand
	{

	}
	public class Chat : AuthenticatedCommand
	{

	}
	public class Console : AuthenticatedCommand
	{

	}

	public struct Authentication
	{
		public int AuthLevel { get; set; }
		public string[] Permissions { get; set; }
		public string[] Groups { get; set; }
		public int Cooldown { get; set; }
	}
}

public class AuthenticatedCommand : BaseCommand
{
	public Authentication Auth { get; set; }

	public override bool CanExecute(Args arg)
	{
		switch (arg)
		{
			case PlayerArgs playerArg:
				{
					if (Auth.Permissions != null)
					{
						var hasPerm = Auth.Permissions.Length == 0;
						foreach (var permission in Auth.Permissions)
						{
							if (Community.Runtime.CorePlugin.permission.UserHasPermission(playerArg.Player.UserIDString, permission))
							{
								hasPerm = true;
								break;
							}
						}

						if (!hasPerm)
						{
							playerArg.Player?.ConsoleMessage($"You don't have any of the required permissions to run this command.");
							return false;
						}
					}

					if (Auth.Groups != null)
					{
						var hasGroup = Auth.Groups.Length == 0;
						foreach (var group in Auth.Groups)
						{
							if (Community.Runtime.CorePlugin.permission.UserHasGroup(playerArg.Player.UserIDString, group))
							{
								hasGroup = true;
								break;
							}
						}

						if (!hasGroup)
						{
							playerArg.Player?.ConsoleMessage($"You aren't in any of the required groups to run this command.");
							return false;
						}
					}

					if (Auth.AuthLevel != -1)
					{
						var hasAuth = playerArg.Player?.Connection.authLevel >= Auth.AuthLevel;

						if (!hasAuth)
						{
							playerArg.Player?.ConsoleMessage($"You don't have the minimum auth level [{Auth.AuthLevel}] required to execute this command [your level: {playerArg.Player?.Connection.authLevel}].");
							return false;
						}
					}

					if (CarbonPlugin.IsCommandCooledDown(playerArg.Player, Name, Auth.Cooldown, true))
					{
						return false;
					}
				}
				break;
		}

		return base.CanExecute(arg);
	}
}

public class PlayerArgs : Args
{
	public BasePlayer Player { get; set; }
}
