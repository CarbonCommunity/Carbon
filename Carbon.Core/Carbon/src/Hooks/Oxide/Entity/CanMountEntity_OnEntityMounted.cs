///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks
{
	[OxideHook("OnEntityMounted"), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("mountable", typeof(BaseMountable))]
	[OxideHook.Info("Called when an entity is mounted by a player.")]
	[OxideHook.Patch(typeof(BaseMountable), "MountPlayer")]
	public class BaseMountable_MountPlayer_OnEntityMounted
	{
		public static bool Prefix(BasePlayer player, ref BaseMountable __instance)
		{
			if (__instance._mounted != null)
			{
				return false;
			}
			if (__instance.mountAnchor == null)
			{
				return false;
			}

			return HookCaller.CallStaticHook("CanMountEntity", player, __instance) == null;
		}

		public static void Postfix(BasePlayer player, ref BaseMountable __instance)
		{
			if (player.isMounted) HookCaller.CallStaticHook("OnEntityMounted", __instance, player);
		}
	}

	[OxideHook("CanMountEntity", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Require("OnEntityMounted")]
	[OxideHook.Parameter("mountable", typeof(BaseMountable))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player attempts to mount an entity.")]
	[OxideHook.Patch(typeof(BaseMountable), "MountPlayer")]
	public class BaseMountable_MountPlayer_CanMountEntity
	{
		public static void Prefix(BasePlayer player) { }
	}
}
