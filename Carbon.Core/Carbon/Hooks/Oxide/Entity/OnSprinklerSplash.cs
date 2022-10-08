///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnSprinklerSplashed"), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(Sprinkler))]
	[OxideHook.Info("Called after any sprinkler has splashed water.")]
	[OxideHook.Patch(typeof(Sprinkler), "DoSplash")]
	public class Sprinkler_DoSplash
	{
		public static void Postfix(ref Sprinkler __instance)
		{
			HookExecutor.CallStaticHook("OnSprinklerSplashed", __instance);
		}
	}
}
