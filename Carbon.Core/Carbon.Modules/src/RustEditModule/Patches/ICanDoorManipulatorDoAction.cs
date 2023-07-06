using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustEditModule
{
	[HookAttribute.Patch("ICanDoorManipulatorDoAction", "ICanDoorManipulatorDoAction", typeof(DoorManipulator), "DoAction", new System.Type[] { })]
	[HookAttribute.Identifier("72f6b7abffcd42e88a8c3b82b9c3f586")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class Entity_DoorManipulator_DoAction_72f6b7abffcd42e88a8c3b82b9c3f586 : API.Hooks.Patch
	{
		public static bool Prefix(DoorManipulator __instance)
		{
			if (HookCaller.CallStaticHook(1871062713, __instance) != null)
			{
				return false;
			}

			return true;
		}
	}
}
