///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnFireworkDesignChanged"), OxideHook.Category(Hook.Category.Enum.Firework)]
	[OxideHook.Parameter("this", typeof(PatternFirework))]
	[OxideHook.Parameter("design", typeof(ProtoBuf.PatternFirework.Design))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called after a firework design is changed.")]
	[OxideHook.Patch(typeof(PatternFirework), "ServerSetFireworkDesign")]
	public class PatternFirework_ServerSetFireworkDesign_OnFireworkDesignChanged
	{
		public static void Postfix (BaseEntity.RPCMessage rpc, ref PatternFirework __instance)
		{
			ProtoBuf.PatternFirework.Design design = ProtoBuf.PatternFirework.Design.Deserialize(rpc.read);

			HookExecutor.CallStaticHook("OnFireworkDesignChanged", __instance, design, rpc.player);
		}
	}
}
