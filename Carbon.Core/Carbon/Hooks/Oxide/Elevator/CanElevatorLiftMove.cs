///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("CanElevatorLiftMove", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Elevator)]
	[OxideHook.Parameter("this", typeof(ElevatorLift))]
	[OxideHook.Info("Called when an elevator lift is attempting to move")]
	[OxideHook.Info("Return true or false to override default behavior")]
	[OxideHook.Patch(typeof(ElevatorLift), "CanMove")]
	public class ElevatorLift_CanMove
	{
		public static bool Prefix(ref ElevatorLift __instance, ref bool __result)
		{
			object returnvar = HookExecutor.CallStaticHook("CanElevatorLiftMove", __instance);

			if (returnvar is bool value)
			{
				__result = value;

				return false;
			}

			return true;
		}
	}
}
