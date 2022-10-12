///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[OxideHook("OnTeamCreate", typeof(object)), OxideHook.Category(Hook.Category.Enum.Team)]
	[OxideHook.Parameter("player", typeof(BasePlayer))]
	[OxideHook.Info("Useful for canceling team creation.")]
	[OxideHook.Patch(typeof(RelationshipManager), "trycreateteam")]
	public class RelationshipManager_trycreateteam_OnTeamCreate
	{
		public static bool Prefix(ConsoleSystem.Arg arg)
		{
			if (RelationshipManager.maxTeamSize == 0)
			{
				arg.ReplyWith("Teams are disabled on this server");
				return false;
			}

			var basePlayer = arg.Player();
			if (basePlayer.currentTeam != 0UL)
			{
				return false;
			}

			if (HookExecutor.CallStaticHook("OnTeamCreate", basePlayer) != null)
			{
				return false;
			}

			var playerTeam = RelationshipManager.ServerInstance.CreateTeam();
			var playerTeam2 = playerTeam;
			playerTeam2.teamLeader = basePlayer.userID;
			playerTeam2.AddPlayer(basePlayer);
			HookExecutor.CallStaticHook("OnTeamCreated", basePlayer, playerTeam);

			return false;
		}
	}
}
