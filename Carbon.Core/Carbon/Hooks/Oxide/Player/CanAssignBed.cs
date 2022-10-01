///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("CanAssignBed", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(SleepingBag))]
	[OxideHook.Parameter("friendId", typeof(ulong))]
	[OxideHook.Info("Called when a player attempts to assign a bed or sleeping bag to another player.")]
	[OxideHook.Patch(typeof(SleepingBag), "AssignToFriend")]
	public class SleepingBag_CanAffordUpgrade
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref SleepingBag __instance)
		{
			if (!msg.player.CanInteract())
			{
				return true;
			}

			if (__instance.deployerUserID != msg.player.userID)
			{
				return true;
			}

			var oldPosition = msg.read.Position;
			var userId = msg.read.UInt64();
			msg.read.Position = oldPosition;

			if (userId == 0UL)
			{
				return true;
			}

			return HookExecutor.CallStaticHook("CanAssignBed", msg.player, __instance, userId) == null;
		}
	}
}
