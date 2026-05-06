namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	private static object IOnCupboardAuthorize(ulong userID, BasePlayer player, BuildingPrivlidge privlidge)
	{
		if (userID == player.userID)
		{
			// OnCupboardAuthorize
			if (HookCaller.CallStaticHook(1460091328, privlidge, player) != null)
			{
				return true;
			}
		}
		// OnCupboardAssign
		else if (HookCaller.CallStaticHook(2217887722, privlidge, userID, player) != null)
		{
			return true;
		}

		return null;
	}
}
