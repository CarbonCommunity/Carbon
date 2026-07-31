#if !MINIMAL

using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_Recycler
	{
		[HookAttribute.Patch("IRecyclerThinkSpeed", "IRecyclerThinkSpeed", typeof(Recycler), "GetRecyclerStats", new System.Type[] { typeof(float), typeof(float) })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IRecyclerThinkSpeed : Patch
		{
			private static void Postfix(Recycler __instance, ref float efficiency, ref float duration)
			{
				if (Community.Runtime.Core.IRecyclerThinkSpeed(__instance) is not float value) return;
				duration *= value;
			}
		}
	}
}

#endif
