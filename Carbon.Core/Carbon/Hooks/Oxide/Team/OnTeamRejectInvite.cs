///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
	[OxideHook("OnTeamRejectInvite", typeof(object)), OxideHook.Category(Hook.Category.Enum.Team)]
	[OxideHook.Parameter("rejector", typeof(BasePlayer))]
	[OxideHook.Parameter("team", typeof(RelationshipManager.PlayerTeam))]
	[OxideHook.Info("Useful for canceling the invitation rejection.")]
	[OxideHook.Patch(typeof(RelationshipManager), "rejectinvite")]
	public class RelationshipManager_rejectinvite
	{
		public static bool Prefix(ConsoleSystem.Arg arg)
		{
			var basePlayer = arg.Player();

			if (basePlayer == null)
			{
				return false;
			}

			if (basePlayer.currentTeam != 0UL)
			{
				return false;
			}

			var @ulong = arg.GetULong(0, 0UL);
			var playerTeam = RelationshipManager.ServerInstance.FindTeam(@ulong);

			if (playerTeam == null)
			{
				basePlayer.ClearPendingInvite();
				return false;
			}

			if (Interface.CallHook("OnTeamRejectInvite", basePlayer, playerTeam) != null)
			{
				return false;
			}

			playerTeam.RejectInvite(basePlayer);
			return false;
		}
	}
}
