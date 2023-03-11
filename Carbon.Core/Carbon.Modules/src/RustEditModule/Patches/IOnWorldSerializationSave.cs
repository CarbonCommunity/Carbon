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
	[HookAttribute.Patch("IOnWorldSerializationSave", "IOnWorldSerializationSave", typeof(WorldSerialization), "Save", new System.Type[] { typeof(string) })]
	[HookAttribute.Identifier("0c164134a9b646bfa8c1085705952205")]
	[HookAttribute.Options(HookFlags.Hidden)]

	// Called before and after WorldSerialization is saved.

	public class World_WorldSerialization_Save_0c164134a9b646bfa8c1085705952205
	{
		public static void Prefix(string fileName, ref WorldSerialization __instance)
		{
			HookCaller.CallStaticHook("IOnWorldSerializationSave", fileName, __instance);
		}

		public static void Postfix(string fileName, ref WorldSerialization __instance)
		{
			HookCaller.CallStaticHook("IOnWorldSerializationSaved", fileName, __instance);
		}
	}
}
