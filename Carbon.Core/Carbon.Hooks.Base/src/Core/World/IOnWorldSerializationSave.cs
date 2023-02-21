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
		[HookAttribute.Identifier("46e7586c847e42b392ec0c14b75b9451")]

		// Called before and after WorldSerialization is saved.

		public class World_WorldSerialization_Save_46e7586c847e42b392ec0c14b75b9451
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
