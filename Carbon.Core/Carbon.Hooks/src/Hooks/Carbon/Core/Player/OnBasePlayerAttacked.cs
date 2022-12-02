/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Player
{
	public partial class Entity_BasePlayer
	{
		/*
		[OxideHook("OnBasePlayerAttacked", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
		[OxideHook.Parameter("this", typeof(BasePlayer))]
		[OxideHook.Parameter("hitInfo", typeof(HitInfo))]
		[OxideHook.Info("Called before the player is being attacked.")]
		[OxideHook.Patch(typeof(BasePlayer), "OnAttacked")]
		*/

		public class Entity_BasePlayer_OnAttacked_4f2d81e4b06f477cb773e7a1cceea710
		{
			public static Metadata metadata = new Metadata("OnBasePlayerAttacked",
				typeof(BasePlayer), "OnAttacked", new System.Type[] { typeof(HitInfo) });

			static Entity_BasePlayer_OnAttacked_4f2d81e4b06f477cb773e7a1cceea710()
			{
				metadata.SetIdentifier("4f2d81e4b06f477cb773e7a1cceea710");
			}

			public static bool Prefix(HitInfo info, ref BasePlayer __instance)
			{
				return HookCaller.CallStaticHook("OnPlayerSleep", __instance, info) == null;
			}
		}
	}
}
