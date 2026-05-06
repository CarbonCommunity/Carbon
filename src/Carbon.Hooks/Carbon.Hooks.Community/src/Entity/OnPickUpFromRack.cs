using API.Hooks;

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class WeaponRack_Entity
	{
		[HookAttribute.Patch("OnPickupFromRack", "OnPickupFromRack", typeof(WeaponRack), "GivePlayerWeapon", new System.Type[] { typeof(BasePlayer), typeof(int), typeof(int), typeof(bool), typeof(bool) })]
		[HookAttribute.Dependencies(new string[] { "CanPickupFromRack" })]

		[MetadataAttribute.Info("Whenever a weapon was picked up from the rack.")]
		[MetadataAttribute.Parameter("rack", typeof(WeaponRack))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Parameter("item", typeof(Item))]
		[MetadataAttribute.Parameter("mountSlotIndex", typeof(int))]
		[MetadataAttribute.Parameter("playerBeltIndex", typeof(int))]
		[MetadataAttribute.Parameter("tryHold", typeof(bool))]

		public class OnPickupFromRack
		{
			// Implemented in CanPickupFromRack
		}
	}
}
