using API.Hooks;
using static BaseEntity;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class ModerationToolsModule
{
	[HookAttribute.Patch("CanUnlockTechTreeNode", "CanUnlockTechTreeNode", typeof(Workbench), "RPC_TechTreeUnlock", new System.Type[] { typeof(RPCMessage) })]
	[HookAttribute.Identifier("7e5b2f14e42c4943bdcdb502d1efb0a0")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Workbench_CanUnlockTechTreeNode_7e5b2f14e42c4943bdcdb502d1efb0a0 : API.Hooks.Patch
	{
		public static bool Prefix(RPCMessage msg, ref Workbench __instance)
		{
			var oldPosition = msg.read.Position;
			var nodeId = msg.read.Int32();
			var node = __instance.techTree.GetByID(nodeId);

			if (HookCaller.CallStaticHook("CanUnlockTechTreeNode", __instance, msg.player, node) is bool result)
			{
				msg.read.Position = oldPosition;
				return result;
			}

			msg.read.Position = oldPosition;
			return true;
		}
	}
}
