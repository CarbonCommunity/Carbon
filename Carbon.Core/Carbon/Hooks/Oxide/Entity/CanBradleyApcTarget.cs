///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
	[OxideHook("CanBradleyApcTarget", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("this", typeof(BradleyAPC))]
	[OxideHook.Parameter("entity", typeof(BaseEntity))]
	[OxideHook.Info("Called when an APC targets an entity.")]
	[OxideHook.Patch(typeof(BradleyAPC), "VisibilityTest")]
	public class BradleyAPC_VisibilityTest
	{
		public static bool Prefix(BaseEntity ent, ref BradleyAPC __instance, out bool __result)
		{
			if (ent == null)
			{
				__result = false;
				return false;
			}

			if (Vector3.Distance(ent.transform.position, __instance.transform.position) >= __instance.viewDistance)
			{
				__result = false;
				return false;
			}

			if (ent is BasePlayer player)
			{
				var position = __instance.mainTurret.transform.position;

				__result = (__instance.IsVisible(player.eyes.position, position, float.PositiveInfinity) || __instance.IsVisible(player.transform.position + Vector3.up * 0.1f, position, float.PositiveInfinity));
				if (!__result && player.isMounted && player.GetMounted().VehicleParent() != null && player.GetMounted().VehicleParent().AlwaysAllowBradleyTargeting)
				{
					__result = __instance.IsVisible(player.GetMounted().VehicleParent().bounds.center, position, float.PositiveInfinity);
				}
				if (__result)
				{
					__result = !UnityEngine.Physics.SphereCast(new Ray(position, Vector3Ex.Direction(player.eyes.position, position)), 0.05f, Vector3.Distance(player.eyes.position, position), 10551297);
				}
			}
			else
			{
				__result = __instance.IsVisible(ent.CenterPoint(), float.PositiveInfinity);
			}

			var obj = Interface.CallHook("CanBradleyApcTarget", __instance, ent);
			if (obj is bool)
			{
				__result = (bool)obj;
			}

			return false;
		}
	}
}
