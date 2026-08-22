using API.Hooks;

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class WeaponRack_Entity
	{
		[HookAttribute.Patch("CanPickupAllFromRack", "CanPickupAllFromRack", typeof(WeaponRack), "GivePlayerAllWeapons", new System.Type[] { typeof(BasePlayer), typeof(int) })]

		[MetadataAttribute.Info("Return false to prevent all weapons from being picked up from the rack.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("mountSlotIndex", typeof(int))]

		public class CanPickupAllFromRack : Patch
		{
			public static bool Prefix(BasePlayer player, int mountSlotIndex, WeaponRack __instance)
			{
				if (player == null)
				{
					return false;
				}

				if (HookCaller.CallStaticHook(2434163304, __instance, player, mountSlotIndex) is bool hookValue)
				{
					return hookValue;
				}

				return true;
			}
		}
	}
}
