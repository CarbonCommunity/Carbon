using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Server
{
	public partial class Server_ConsoleSystem
	{
		[HookAttribute.Patch("OnNativeCommandHasPermission", "OnNativeCommandHasPermission", typeof(ConsoleSystem.Arg), "HasPermission", new System.Type[] { })]
		[HookAttribute.Identifier("708ba01d21fa4d33ae66bd25fe7390c1")]
		[HookAttribute.Options(HookFlags.None)]

		[MetadataAttribute.Info("Overrides Rust's native checks for command execution authorization.")]
		[MetadataAttribute.Info("Example: allow `inventory.give` to be executed by regular players.")]
		[MetadataAttribute.Parameter("arg", typeof(ConsoleSystem.Arg))]
		[MetadataAttribute.Return(typeof(bool))]

		public class Static_ConsoleSystem_708ba01d21fa4d33ae66bd25fe7390c1 : Patch
		{
			public static bool Prefix(ref ConsoleSystem.Arg __instance, ref bool __result)
			{
				if (HookCaller.CallStaticHook(2656314715, __instance) is bool value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}
	}
}
