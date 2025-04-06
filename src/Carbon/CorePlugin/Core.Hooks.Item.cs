namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnLoseCondition(Item item, float amount)
	{
		var args = HookCaller.Caller.AllocateBuffer(2);

		args[0] = item;
		args[1] = amount;

		// OnLoseCondition
		HookCaller.CallStaticHook(2025192851, args: args);
		amount = (float)args[1];

		HookCaller.Caller.ReturnBuffer(args);

		var condition = item.condition;
		item.condition -= amount;
		if (item.condition <= 0f && item.condition < condition)
		{
			item.OnBroken();
		}

		return Cache.True;
	}
}
