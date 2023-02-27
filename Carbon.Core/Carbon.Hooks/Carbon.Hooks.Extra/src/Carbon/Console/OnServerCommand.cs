﻿using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ConsoleSystem
	{
		[HookAttribute.Patch("OnServerCommand", typeof(ConsoleSystem), "Internal", new System.Type[] { typeof(ConsoleSystem.Arg) })]
		[HookAttribute.Identifier("c12426936931457aa7f9cdf6db1a1127")]

		// Useful for intercepting commands before they get to their intended target.

		public class Static_ConsoleSystem_ServerConsoleCommand_c12426936931457aa7f9cdf6db1a1127 : Patch
		{
			public static bool Prefix(ConsoleSystem.Arg arg)
			{
				if (arg.Invalid) return false;
				return HookCaller.CallStaticHook("OnServerCommand", arg) == null;
			}
		}
	}
}