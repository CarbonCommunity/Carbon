using Connection = Network.Connection;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	private static readonly DateTime Eoy = new(2025, 12, 31);

	private void OnTeamMemberPromote(RelationshipManager.PlayerTeam team, ulong newTeamLeader)
	{
		if (BasePlayer.FindByID(newTeamLeader) is BasePlayer player && player.IsValid())
		{
			// OnTeamPromote aka 3930614067
			// OnTeamMemberPromote aka 1658239813
			HookCaller.CallStaticDeprecatedHook(3930614067, 1658239813, Eoy, team, player);
		}
	}

	internal static object IOnTeamInvite(BasePlayer basePlayer, BasePlayer basePlayer2)
	{
		// OnTeamInvite aka 2072886543
		// OnTeamMemberInvite aka 844539354
		return HookCaller.CallStaticDeprecatedHook(2072886543, 844539354, Eoy, basePlayer, basePlayer2);
	}
}
