///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks
{
	[OxideHook("OnEntityDismounted"), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("mountable", typeof(BaseMountable))]
	[OxideHook.Info("Called when an entity is dismounted of a player.")]
	[OxideHook.Patch(typeof(BaseMountable), "DismountPlayer")]
	public class BaseMountable_DismountPlayer_OnEntityDismount
	{
		public static bool Prefix(BasePlayer player, bool lite, ref BaseMountable __instance)
		{
			if (__instance._mounted == null)
			{
				return false;
			}
			if (__instance.mountAnchor == null)
			{
				return false;
			}

			return HookCaller.CallStaticHook("CanDismountEntity", player, __instance) == null;
		}

		public static void Postfix(BasePlayer player, bool lite, ref BaseMountable __instance)
		{
			if (!player.isMounted) HookCaller.CallStaticHook("OnEntityDismounted", __instance, player);
		}
	}

	[OxideHook("CanDismountEntity", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Require("OnEntityDismounted")]
	[OxideHook.Parameter("mountable", typeof(BaseMountable))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player attempts to mount an entity.")]
	[OxideHook.Patch(typeof(BaseMountable), "DismountPlayer")]
	public class BaseMountable_DismountPlayer_CanDismountEntity
	{
		public static void Prefix(BasePlayer player, bool lite) { }
	}
}
