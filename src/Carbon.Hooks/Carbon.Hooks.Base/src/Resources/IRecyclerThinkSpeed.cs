#if !MINIMAL

using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_Recycler
	{
		[HookAttribute.Patch("IRecyclerThinkSpeed", "IRecyclerThinkSpeed", typeof(Recycler), "GetRecycleThinkDuration", new System.Type[] { })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IRecyclerThinkSpeed : Patch
		{
			private static void Postfix(Recycler __instance, ref float __result)
			{
				if (Community.Runtime.Core.IRecyclerThinkSpeed(__instance) is not float value) return;
				__result *= value;
			}
		}
	}
}

#endif
