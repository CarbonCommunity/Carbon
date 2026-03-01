#if !MINIMAL

using API.Hooks;

namespace Carbon.Modules;

public partial class AdminModule
{
	[HookAttribute.Patch("IModBackpack", "IModBackpack", typeof(ItemModBackpack), "CanAcceptItem", new System.Type[] { typeof(Item), typeof(Item), typeof(int) })]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Item_ModBackpack : API.Hooks.Patch
	{
		public static bool Prefix(Item backpack, Item item, int slot, ref bool __result)
		{
			if (AcceptOnBackpack(backpack))
			{
				__result = true;
				return false;
			}

			return true;
		}
	}
}

#endif
