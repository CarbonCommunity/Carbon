using System.IO;
using System.Reflection;
using Carbon.Utility;
using Patches;
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
		string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		Logger.Log($">> {assemblyName} is using UnityDoorstop entrypoint");

		// this forces Harmony v2 to be loaded instead of the rust's builtin v1
		Assembly assembly = Assembly.LoadFile(Path.Combine(Context.CarbonLib, "0Harmony.dll"));
		Logger.Log($"Loaded {assembly.GetName().Name} {assembly.GetName().Version} into current AppDomain");

		using Sandbox<AssemblyCSharp> isolated1 = new Sandbox<AssemblyCSharp>();

		if (!isolated1.Do.IsPublic("ServerMgr", "Shutdown"))
		{
			isolated1.Do.Publicize();
			isolated1.Do.Patch();
			isolated1.Do.Write();

			using Sandbox<RustHarmony> isolated2 = new Sandbox<RustHarmony>();
			isolated2.Do.Patch();
			isolated2.Do.Write();
		}
	}
}
