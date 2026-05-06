#if !MINIMAL

using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_VendingMachine
	{
		[HookAttribute.Patch("IVendingBuyDuration", "IVendingBuyDuration", typeof(VendingMachine), "GetBuyDuration", new System.Type[] { })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IVendingBuyDuration1 : Patch
		{
			public static void Postfix(VendingMachine __instance, ref float __result)
			{
				if (Community.Runtime.Core.IVendingBuyDuration() is not float value) return;
				__result *= value;
			}
		}

		[HookAttribute.Patch("IVendingBuyDuration", "IVendingBuyDuration", typeof(InvisibleVendingMachine), "GetBuyDuration", new System.Type[] { })]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class IVendingBuyDuration2 : Patch
		{
			public static void Postfix(InvisibleVendingMachine __instance, ref float __result)
			{
				if (Community.Runtime.Core.IVendingBuyDuration() is not float value) return;
				__result *= value;
			}
		}
	}
}

#endif
