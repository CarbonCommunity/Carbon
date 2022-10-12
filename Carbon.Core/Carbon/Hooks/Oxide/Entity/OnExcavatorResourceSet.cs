///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnExcavatorResourceSet", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("arm", typeof(ExcavatorArm))]
	[OxideHook.Parameter("resourceName", typeof(string))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player is trying to set a new resource target.")]
	[OxideHook.Patch(typeof(ExcavatorArm), "RPC_SetResourceTarget")]
	public class ExcavatorArm_RPC_SetResourceTarget
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref ExcavatorArm __instance)
		{
			var oldPosition = msg.read.Position;
			var text = msg.read.String(256);
			if (Interface.CallHook("OnExcavatorResourceSet", __instance, text, msg.player) != null)
			{
				return false;
			}
			msg.read.Position = oldPosition;
			return true;
		}
	}
}
