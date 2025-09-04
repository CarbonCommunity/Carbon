namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnTeamInvite(BasePlayer sender, BasePlayer receiver)
	{
		// Fire OnTeamInvite when the old OnTeamMemberInvite becomes deprecated
		return null;
	}
}
