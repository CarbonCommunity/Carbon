///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Network;
using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnAnalysisComplete", typeof(bool)), OxideHook.Category(Hook.Category.Enum.Player)]
	[OxideHook.Parameter("crater", typeof(SurveyCrater))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called right after a player completes a survey crater analysis.")]
	[OxideHook.Patch(typeof(SurveyCrater), "AnalysisComplete")]
	public class SurveyCrater_AnalysisComplete
	{
		public static void Prefix(BaseEntity.RPCMessage msg, ref SurveyCrater __instance)
		{
			Interface.CallHook("OnAnalysisComplete", __instance, msg.player);
		}
	}
}
