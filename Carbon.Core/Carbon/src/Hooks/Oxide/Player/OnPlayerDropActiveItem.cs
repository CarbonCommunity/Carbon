///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnPlayerDropActiveItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(BaseCombatEntity))]
	[OxideHook.Info("Called when the player drops their active held item.")]
	[OxideHook.Patch(typeof(PlayerBelt), "DropActive")]
	public class PlayerBelt_DropActive
	{
		public static bool Prefix(Vector3 position, Vector3 velocity, ref PlayerBelt __instance)
		{
			var activeItem = __instance.player.GetActiveItem();

			if (activeItem == null)
			{
				return false;
			}

			return Interface.CallHook("OnPlayerDropActiveItem", __instance.player, activeItem) == null;
		}
	}
}
