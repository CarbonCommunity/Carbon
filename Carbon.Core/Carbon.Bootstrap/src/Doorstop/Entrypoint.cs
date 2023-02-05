using System;
using System.Reflection;
using Patches.Cecil;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Doorstop;

public sealed class Entrypoint
{
	public static void Start()
	{
		Logger.Log(">> Carbon.Bootstrap is using UnityDoorstop entrypoint");

		using Sandbox<AssemblyCSharp> isolated = new Sandbox<AssemblyCSharp>();

		if (!isolated.Do.IsPublic("ServerMgr", "Shutdown"))
		{
			isolated.Do.Publicize();
			isolated.Do.Cleanup();
			isolated.Do.Write();
		}

		using Sandbox<RustHarmony> isolated2 = new Sandbox<RustHarmony>();

		isolated2.Do.Cleanup();
		isolated2.Do.Write();

		Logger.Log("AFTER ---------------");
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			Logger.Log($" - {assembly.GetName().Name} {assembly.GetName().Version}");
	}
}
