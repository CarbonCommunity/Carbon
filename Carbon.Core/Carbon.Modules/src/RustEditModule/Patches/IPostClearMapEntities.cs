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
	[HookAttribute.Patch("IPostClearMapEntities", "IPostClearMapEntities", typeof(SaveRestore), "ClearMapEntities", new System.Type[] { })]
	[HookAttribute.Identifier("ad75e3d055c341599e960de60049bfcc")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class World_SaveRestore_ClearMapEntities_ad75e3d055c341599e960de60049bfcc : API.Hooks.Patch
	{
		public static void Postfix()
		{
			HookCaller.CallStaticHook("IPostClearMapEntities");
		}
	}
}
