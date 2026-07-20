using API.Hooks;

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class WeaponRack_Entity
	{
		[HookAttribute.Patch("CanPickupAllFromRack", "CanPickupAllFromRack", typeof(WeaponRack), "GivePlayerAllWeapons", new System.Type[] { typeof(BasePlayer), typeof(int) })]

		[MetadataAttribute.Info("Returning a non-null value disallows all weapons to be picked up from the rack.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("mountSlotIndex", typeof(int))]
		[MetadataAttribute.Return(typeof(bool))]

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
