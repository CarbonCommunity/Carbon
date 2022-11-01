///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Hooks
{
	[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
	[CarbonHook("IInit"), CarbonHook.Category(Hook.Category.Enum.Core)]
	[CarbonHook.Patch(typeof(Bootstrap), "StartupShared")]
	public class Init
	{
		public static void Prefix()
		{
			Community.Runtime.Initialize();
		}
	}
}
