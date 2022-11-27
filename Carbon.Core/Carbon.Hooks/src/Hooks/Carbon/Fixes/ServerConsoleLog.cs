using UnityEngine;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fixes
{
	public partial class Fixes_ServerConsole
	{
		/*
		[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
		[CarbonHook("IServerConsoleLog"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Patch(typeof(ServerConsole), "HandleLog")]
		*/
		public class Fixes_ServerConsole_HandleLog_e540e379423c44b9b5855cff2506d5d1
		{
			public static Metadata metadata = new Metadata("IServerConsoleLog",
				typeof(ServerConsole), "HandleLog", new System.Type[] { typeof(string), typeof(string), typeof(UnityEngine.LogType) });

			static Fixes_ServerConsole_HandleLog_e540e379423c44b9b5855cff2506d5d1()
			{
				metadata.SetAlwaysPatch(true);
				metadata.SetHidden(true);
			}

			public static bool Prefix(string message, string stackTrace, LogType type)
			{
				if (message.StartsWith("Trying to load assembly: DynamicAssembly")) return false;

				return true;
			}
		}
	}
}