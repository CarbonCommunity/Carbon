///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;

namespace Carbon.Hooks
{
	[OxideHook("OnTeamUpdate", typeof(object)), OxideHook.Category(Hook.Category.Enum.Team)]
	[OxideHook.Parameter("currentTeam", typeof(ulong))]
	[OxideHook.Parameter("newTeam", typeof(ulong))]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Called when player's team is updated.")]
	[OxideHook.Patch(typeof(BasePlayer), "UpdateTeam")]
	public class BasePlayer_UpdateTeam
	{
		public static bool Prefix(ulong newTeam, ref BasePlayer __instance)
		{
			return Interface.CallHook("OnTeamUpdate", __instance.currentTeam, newTeam, __instance) == null;
		}
	}
}
