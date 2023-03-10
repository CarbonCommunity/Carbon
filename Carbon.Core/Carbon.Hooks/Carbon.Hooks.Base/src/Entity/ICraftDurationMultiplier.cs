using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fixes
{
	public partial class Fixes_CargoShip
	{
		[HookAttribute.Patch("IOnCargoShipEgressStart", "IOnCargoShipEgressStart", typeof(CargoShip), "StartEgress", new System.Type[] { })]
		[HookAttribute.Identifier("153555ffaa5543049c0e05f5b197e697")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_CargoShip_StartEgress_153555ffaa5543049c0e05f5b197e697 : API.Hooks.Patch
		{
			public static void Prefix(CargoShip __instance)
			{
				HookCaller.CallStaticHook("IOnCargoShipEgressStart", __instance);
			}
		}
	}
}
