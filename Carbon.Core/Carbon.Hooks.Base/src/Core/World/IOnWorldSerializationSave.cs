using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_World
{
	public partial class World_WorldSerialization
	{
		[HookAttribute.Patch("IOnWorldSerializationSave", typeof(WorldSerialization), "Save", new System.Type[] { typeof(string) })]
		[HookAttribute.Identifier("0c164134a9b646bfa8c1085705952205")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]

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
}
