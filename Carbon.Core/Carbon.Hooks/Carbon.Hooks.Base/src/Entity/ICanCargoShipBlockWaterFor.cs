using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class Entity_CargoShip
	{
		[HookAttribute.Patch("ICanCargoShipBlockWaterFor", "ICanCargoShipBlockWaterFor", typeof(CargoShip), "BlocksWaterFor", new System.Type[] { })]
		[HookAttribute.Identifier("441be9f49c4e45e89ad5e4b7cd32c33b")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Entity_CargoShip_BlocksWaterFor_441be9f49c4e45e89ad5e4b7cd32c33b : API.Hooks.Patch
		{
			public static bool Prefix(ref bool __result, CargoShip __instance)
			{
				if(HookCaller.CallStaticHook("ICanCargoShipBlockWaterFor", __instance) != null)
				{
					__result = false;
					return false;
				}

				return true;
			}
		}
	}
}
