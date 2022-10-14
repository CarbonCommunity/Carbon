///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnElevatorButtonPress", typeof(object)), OxideHook.Category(Hook.Category.Enum.Elevator)]
	[OxideHook.Parameter("this", typeof(ElevatorLift))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("direction", typeof(Elevator.Direction))]
	[OxideHook.Parameter("upwards", typeof(bool))]
	[OxideHook.Info("Called when a player presses a button on an elevator lift")]
	[OxideHook.Info("Return a non-null value to override default behavior")]
	[OxideHook.Patch(typeof(ElevatorLift), "Server_RaiseLowerFloor")]
	public class ElevatorLift_Server_RaiseLowerFloor
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref ElevatorLift __instance)
		{
			if (!__instance.CanMove())
			{
				return false;
			}

			Elevator.Direction direction = (Elevator.Direction)msg.read.Int32();

			bool flag = msg.read.Bit();

			return HookCaller.CallStaticHook("OnElevatorButtonPress", __instance, msg.player, direction, flag) == null;
		}
	}
}
