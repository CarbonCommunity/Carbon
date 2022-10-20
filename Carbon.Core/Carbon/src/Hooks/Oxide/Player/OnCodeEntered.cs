///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnCodeEntered", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("lock", typeof(CodeLock))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("code", typeof(string))]
	[OxideHook.Info("Called when the player has entered a code in a codelock.")]
	[OxideHook.Patch(typeof(CodeLock), "UnlockWithCode")]
	public class CodeLock_UnlockWithCode
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref CodeLock __instance)
		{
			if (!rpc.player.CanInteract())
			{
				return false;
			}
			if (!__instance.IsLocked())
			{
				return false;
			}
			if (__instance.IsCodeEntryBlocked())
			{
				return false;
			}

			var oldPosition = rpc.read.Position;
			var text = rpc.read.String(256);
			rpc.read.Position = oldPosition;

			return Interface.CallHook("OnCodeEntered", __instance, rpc.player, text) == null;
		}
	}
}
