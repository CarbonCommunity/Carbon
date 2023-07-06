using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class AdminModule
{
	[HookAttribute.Patch("IValidDismountPosition", "IValidDismountPosition", typeof(BaseMountable), "ValidDismountPosition", new System.Type[] { typeof(BasePlayer), typeof(Vector3) })]
	[HookAttribute.Identifier("cb1bb2048c2946d8ad469bc218193a7a")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BaseMountable_ValidDismountPosition_cb1bb2048c2946d8ad469bc218193a7a : API.Hooks.Patch
	{
		public static bool Prefix(BasePlayer player, Vector3 disPos, BaseMountable __instance, ref bool __result)
		{
			if (HookCaller.CallStaticHook(3269023868, __instance, player) is bool hookResult)
			{
				__result = hookResult;
				return false;
			}

			return true;
		}
	}
}
