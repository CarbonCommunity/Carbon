///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnFireworkDesignChange", typeof(object)), OxideHook.Category(Hook.Category.Enum.Firework)]
	[OxideHook.Parameter("this", typeof(PatternFirework))]
	[OxideHook.Parameter("design", typeof(ProtoBuf.PatternFirework.Design))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called before a firework design is changed.")]
	[OxideHook.Patch(typeof(PatternFirework), "ServerSetFireworkDesign")]
	public class PatternFirework_ServerSetFireworkDesign_OnFireworkDesignChange
	{
		public static bool Prefix(BaseEntity.RPCMessage rpc, ref PatternFirework __instance)
		{
			if (!__instance.PlayerCanModify(rpc.player))
			{
				return false;
			}

			ProtoBuf.PatternFirework.Design design = ProtoBuf.PatternFirework.Design.Deserialize(rpc.read);

			return HookExecutor.CallStaticHook("OnFireworkDesignChange", __instance, design, rpc.player) == null;
		}
	}
}
