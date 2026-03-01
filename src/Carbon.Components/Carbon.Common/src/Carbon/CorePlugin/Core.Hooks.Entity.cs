namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static readonly string OnEntitySaved = "OnEntitySaved";

	internal static object IOnEntitySaved(BaseNetworkable baseNetworkable, BaseNetworkable.SaveInfo saveInfo)
	{
		if (!Community.IsServerInitialized || saveInfo.forConnection == null || InternalHooks.OnEntitySaved == 0)
		{
			return null;
		}

		// OnEntitySaved
		HookCaller.CallStaticHook(825712380, baseNetworkable, saveInfo);

		return null;
	}
}
