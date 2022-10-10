///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

[Hook.AlwaysPatched, Hook.Hidden]
[Hook("IInit"), Hook.Category(Hook.Category.Enum.Core)]
[Hook.Patch(typeof(Bootstrap), "StartupShared")]
public class Init
{
	public static void Prefix()
	{
		CarbonCore.Instance.Init();
	}
}
