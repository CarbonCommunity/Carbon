using System.Threading;
using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustEditModule
{
	[HookAttribute.Patch("IPreObjectSetHierarchyGroup", "IPreObjectSetHierarchyGroup", typeof(World), "Spawn", new System.Type[] { typeof(GameObject), typeof(string), typeof(bool), typeof(bool) })]
	[HookAttribute.Identifier("1592e4baaa2847c6a774e84cbbf47f9b")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class World_GameObjectEx_Spawn_1592e4baaa2847c6a774e84cbbf47f9b : API.Hooks.Patch
	{
		public static void Prefix(GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			HookCaller.CallStaticHook("IPreObjectSetHierarchyGroup", obj, strRoot, groupActive, persistant);
		}
	}
}
