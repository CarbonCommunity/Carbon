///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Extended
{
	[OxideHook("OnMagazineReload", typeof(object)), OxideHook.Category(Hook.Category.Enum.Weapon)]
	[OxideHook.Parameter("weapon", typeof(BaseProjectile))]
	[OxideHook.Parameter("desiredAmount", typeof(int))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player reloads a magazine.")]
	[OxideHook.Patch(typeof(BaseProjectile), "ReloadMagazine")]
	public class BaseProjectile_ReloadMagazine
	{
		public static bool Prefix (int desiredAmount, ref BaseProjectile __instance)
		{
			var ownerPlayer = __instance.GetOwnerPlayer();
			if (ownerPlayer == null) return false;

			return Interface.CallHook("OnMagazineReload", __instance, desiredAmount, ownerPlayer) == null;
		}
	}
}
