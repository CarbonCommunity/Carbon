///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnElevatorMove", typeof(object)), OxideHook.Category(Hook.Category.Enum.Elevator)]
	[OxideHook.Parameter("this", typeof(Elevator))]
	[OxideHook.Parameter("targetFloor", typeof(int))]
	[OxideHook.Info("Called right before an elevator starts moving to the target floor")]
	[OxideHook.Info("Return a non-null value to override default behavior")]
	[OxideHook.Patch(typeof(Elevator), "RequestMoveLiftTo")]
	public class Elevator_RequestMoveLiftTo
	{
		public static bool Prefix(int targetFloor, float timeToTravel, ref Elevator __instance, ref bool __result)
		{
			timeToTravel = 0f;

			if (HookExecutor.CallStaticHook("OnElevatorMove", __instance, targetFloor) != null)
			{
				__result = false;

				return false;
			}

			return true;
		}
	}
}
