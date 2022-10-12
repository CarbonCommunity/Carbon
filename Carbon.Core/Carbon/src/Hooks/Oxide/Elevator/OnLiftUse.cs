///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnLiftUse", typeof(object)), OxideHook.Category(Hook.Category.Enum.Elevator)]
	[OxideHook.Parameter("this", typeof(Lift))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player calls a lift or procedural lift")]
	[OxideHook.Info("Return a non-null value to override default behavior")]
	[OxideHook.Patch(typeof(Lift), "RPC_UseLift")]
	public class Lift_RPC_UseLift
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref Lift __instance)
		{
			if (!rpc.player.CanInteract())
			{
				return false;
			}

			return HookExecutor.CallStaticHook("OnLiftUse", __instance, rpc.player) == null;
		}
	}

	[OxideHook("OnLiftUse", typeof(object)), OxideHook.Category(Hook.Category.Enum.Elevator)]
	[OxideHook.Parameter("this", typeof(ProceduralLift))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when a player calls a lift or procedural lift")]
	[OxideHook.Info("Return a non-null value to override default behavior")]
	[OxideHook.Patch(typeof(ProceduralLift), "RPC_UseLift")]
	public class ProceduralLift_RPC_UseLift
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref ProceduralLift __instance)
		{
			if (!rpc.player.CanInteract())
			{
				return false;
			}

			return HookExecutor.CallStaticHook("OnLiftUse", __instance, rpc.player) == null;
		}
	}
}
