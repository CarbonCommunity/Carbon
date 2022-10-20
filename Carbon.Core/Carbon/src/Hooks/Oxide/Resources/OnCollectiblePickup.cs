///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnCollectiblePickup", typeof(object)), OxideHook.Category(Hook.Category.Enum.Resources)]
	[OxideHook.Parameter("this", typeof(CollectibleEntity))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when the player collects an item.")]
	[OxideHook.Patch(typeof(CollectibleEntity), "DoPickup")]
	public class CollectibleEntity_DoPickup
	{
		public static bool Prefix(BasePlayer reciever, ref CollectibleEntity __instance)
		{
			if (__instance.itemList == null)
			{
				return false;
			}

			return HookCaller.CallStaticHook("OnCollectiblePickup", __instance, reciever) == null;
		}
	}
}
