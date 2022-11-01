///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("CanNetworkTo", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(BaseNetworkable))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when an entity attempts to network with a player.")]
	[OxideHook.Info("For better performance, avoid using heavy calculations in this hook.")]
	[OxideHook.Patch(typeof(BaseNetworkable), "ShouldNetworkTo")]
	public class BaseNetworkable_ShouldNetworkTo
	{
		public static bool Prefix(BasePlayer player, ref BaseNetworkable __instance, out bool __result)
		{
			__result = default;

			var obj = Interface.CallHook("CanNetworkTo", __instance, player);

			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanNetworkTo", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(BasePlayer))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when an entity attempts to network with a player.")]
	[OxideHook.Info("For better performance, avoid using heavy calculations in this hook.")]
	[OxideHook.Patch(typeof(BasePlayer), "ShouldNetworkTo")]
	public class BasePlayer_ShouldNetworkTo
	{
		public static bool Prefix(BasePlayer player, ref BasePlayer __instance, out bool __result)
		{
			__result = default;

			var obj = Interface.CallHook("CanNetworkTo", __instance, player);

			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			return true;
		}
	}

	[OxideHook("CanNetworkTo", typeof(object)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(BaseEntity))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when an entity attempts to network with a player.")]
	[OxideHook.Info("For better performance, avoid using heavy calculations in this hook.")]
	[OxideHook.Patch(typeof(BaseEntity), "ShouldNetworkTo")]
	public class BaseEntity_ShouldNetworkTo
	{
		public static bool Prefix(BasePlayer player, ref BaseEntity __instance, out bool __result)
		{
			if (player == __instance)
			{
				__result = true;
				return false;
			}

			var parentEntity = __instance.GetParentEntity();
			if (__instance.limitNetworking)
			{
				if (parentEntity == null)
				{
					__result = false;
					return false;
				}
				if (parentEntity != player)
				{
					__result = false;
					return false;
				}
			}
			if (!(parentEntity != null))
			{
				__result = _base_ShouldNetworkTo(__instance, player);
				return false;
			}

			var obj = Interface.CallHook("CanNetworkTo", __instance, player);
			if (obj is bool)
			{
				__result = (bool)obj;
				return false;
			}

			__result = parentEntity.ShouldNetworkTo(player);
			return false;
		}

		internal static bool _base_ShouldNetworkTo(BaseEntity entity, BasePlayer player)
		{
			var obj = Interface.CallHook("CanNetworkTo", entity, player);

			if (obj is bool)
			{
				return (bool)obj;
			}

			return entity.net.group == null || player.net.subscriber.IsSubscribed(entity.net.group);
		}
	}
}
