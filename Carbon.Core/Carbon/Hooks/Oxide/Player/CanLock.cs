///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanLock", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(KeyLock))]
	[OxideHook.Info("Useful for canceling the lock action.")]
	[OxideHook.Patch(typeof(KeyLock), "Lock")]
	public class KeyLock_Lock
	{
		public static bool Prefix(BasePlayer player, ref KeyLock __instance)
		{
			if (player == null)
			{
				return true;
			}

			if (!player.CanInteract())
			{
				return true;
			}

			if (__instance.IsLocked())
			{
				return true;
			}

			return HookExecutor.CallStaticHook("CanLock", player, __instance) == null;
		}
	}

	[OxideHook("CanLock", typeof(object)), Hook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(CodeLock))]
	[OxideHook.Info("Useful for canceling the lock action.")]
	[OxideHook.Patch(typeof(CodeLock), "TryLock")]
	public class CodeLock_Lock
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref CodeLock __instance)
		{
			if (!rpc.player.CanInteract())
			{
				return true;
			}
			if (__instance.IsLocked())
			{
				return true;
			}
			if (__instance.code.Length != 4)
			{
				return true;
			}

			return HookExecutor.CallStaticHook("CanLock", rpc.player, __instance) == null;
		}
	}
}
