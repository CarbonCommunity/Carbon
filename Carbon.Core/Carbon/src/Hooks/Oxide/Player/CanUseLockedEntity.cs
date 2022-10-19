///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanUseLockedEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(SurveyCrater))]
	[OxideHook.Parameter("entity", typeof(KeyLock))]
	[OxideHook.Info("Called when the player tries to use an entity that is locked.")]
	[OxideHook.Patch(typeof(KeyLock), "OnTryToClose")]
	public class KeyLock_OnTryToClose
	{
		public static bool Prefix(BasePlayer player, ref KeyLock __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanUseLockedEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanUseLockedEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(SurveyCrater))]
	[OxideHook.Parameter("entity", typeof(CodeLock))]
	[OxideHook.Info("Called when the player tries to use an entity that is locked.")]
	[OxideHook.Patch(typeof(CodeLock), "OnTryToClose")]
	public class CodeLock_OnTryToClose
	{
		public static bool Prefix(BasePlayer player, ref CodeLock __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanUseLockedEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanUseLockedEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(SurveyCrater))]
	[OxideHook.Parameter("entity", typeof(KeyLock))]
	[OxideHook.Info("Called when the player tries to use an entity that is locked.")]
	[OxideHook.Patch(typeof(KeyLock), "OnTryToOpen")]
	public class KeyLock_OnTryToOpen
	{
		public static bool Prefix(BasePlayer player, ref KeyLock __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanUseLockedEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanUseLockedEntity", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(SurveyCrater))]
	[OxideHook.Parameter("entity", typeof(CodeLock))]
	[OxideHook.Info("Called when the player tries to use an entity that is locked.")]
	[OxideHook.Patch(typeof(CodeLock), "OnTryToOpen")]
	public class CodeLock_OnTryToOpen
	{
		public static bool Prefix(BasePlayer player, ref CodeLock __instance, out bool __result)
		{
			__result = default;

			var result = Interface.CallHook("CanUseLockedEntity", player, __instance);

			if (result is bool boolResult)
			{
				__result = boolResult;
				return false;
			}

			return true;
		}
	}
}
