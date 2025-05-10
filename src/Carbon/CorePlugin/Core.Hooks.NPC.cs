using Rust.Ai.Gen2;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnNpcTarget(SenseComponent sense, BaseEntity target)
	{
		if (!sense || !target)
		{
			return null;
		}

		var baseEntity = sense.baseEntity;

		if (baseEntity == null)
		{
			return null;
		}

		if (HookCaller.CallStaticHook(1066895325, baseEntity, target) != null)
		{
			return Cache.False;
		}

		return null;
	}
}
