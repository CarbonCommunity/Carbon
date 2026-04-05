using API.Hooks;

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class WeaponRack_Entity
	{
		[HookAttribute.Patch("CanPlaceOnRack", "CanPlaceOnRack", typeof(WeaponRack), "MountWeapon", new System.Type[] { typeof(Item), typeof(BasePlayer), typeof(int), typeof(int), typeof(bool) })]

		[MetadataAttribute.Info("Returning a non-null value disallows the weapon to be placed.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Parameter("gridCellIndex", typeof(int))]
		[MetadataAttribute.Parameter("rotation", typeof(int))]
		[MetadataAttribute.Return(typeof(bool))]

		public class CanPlaceOnRack : Patch
		{
			public static bool Prefix(Item item, BasePlayer player, int gridCellIndex, int rotation, bool sendUpdate, ref bool __result, WeaponRack __instance)
			{
				if (HookCaller.CallStaticHook(1860967996, __instance, player, item, gridCellIndex, rotation) is bool hookValue)
				{
					__result = hookValue;
					return false;
				}

				return true;
			}
		}
	}
}
