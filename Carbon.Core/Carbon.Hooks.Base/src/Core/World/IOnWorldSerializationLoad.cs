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
		[HookAttribute.Patch("IOnWorldSerializationLoad", typeof(WorldSerialization), "Load", new System.Type[] { typeof(string) })]
		[HookAttribute.Identifier("46e7586c847e42b392ec0c14b75b9451")]

		// Called before and after WorldSerialization is loaded.

		public class World_WorldSerialization_Load_46e7586c847e42b392ec0c14b75b9451
		{
			public static void Prefix(string fileName, ref WorldSerialization __instance)
			{
				HookCaller.CallStaticHook("IOnWorldSerializationLoad", fileName, __instance);
			}

			public static void Postfix(string fileName, ref WorldSerialization __instance)
			{
				HookCaller.CallStaticHook("IOnWorldSerializationLoaded", fileName, __instance);
			}
		}
	}
}
