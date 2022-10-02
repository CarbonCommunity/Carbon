///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
	[OxideHook("CanWaterBallSplash", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Entity)]
	[OxideHook.Parameter("liquidDef", typeof(ItemDefinition))]
	[OxideHook.Parameter("position", typeof(Vector3))]
	[OxideHook.Parameter("radius", typeof(float))]
	[OxideHook.Parameter("amount", typeof(int))]
	[OxideHook.Info("Called before water is poured from a liquid vessel or shot from a water gun")]
	[OxideHook.Patch(typeof(WaterBall), "DoSplash", typeof(Vector3), typeof(float), typeof(ItemDefinition), typeof(int))]
	public class WaterBall_DoSplash
	{
		public static bool Prefix(Vector3 position, float radius, ItemDefinition liquidDef, int amount, ref bool __result)
		{
			var obj = Interface.CallHook("CanWaterBallSplash", liquidDef, position, radius, amount);

			if (obj is bool)
			{
				__result = (bool)obj;
				return __result;
			}

			return true;
		}
	}
}
