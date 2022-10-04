///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("CanDeployItem", typeof(object)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Parameter("this", typeof(Deployer))]
	[OxideHook.Parameter("entityId", typeof(uint))]
	[OxideHook.Info("Useful for denying items' deployment.")]
	[OxideHook.Patch(typeof(Deployer), "DoDeploy")]
	public class Deployer_DoDeploy
	{
		public static bool Prefix(BaseEntity.RPCMessage msg, ref Deployer __instance)
		{
			if (!msg.player.CanInteract())
			{
				return true;
			}

			var deployable = __instance.GetDeployable();

			if (deployable == null)
			{
				return true;
			}

			var oldPosition = msg.read.Position;
			var ray = msg.read.Ray();
			var num = msg.read.UInt32();
			msg.read.Position = oldPosition;

			return HookExecutor.CallStaticHook("CanDeployItem", msg.player, __instance, num) == null;
		}
	}
}
