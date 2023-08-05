using System.IO;
using System.Reflection;
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
	private static readonly string[] Preload = {
		Path.Combine(Context.CarbonLib, "0Harmony.dll"),
	};

	public static void Start()
	{
		string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		Logger.Log($">> {assemblyName} is using a mono injector as entrypoint");

		using Sandbox<AssemblyCSharp> isolated1 = new Sandbox<AssemblyCSharp>();
		{
			if (!isolated1.Do.IsPublic("ServerMgr", "Shutdown"))
			{
				isolated1.Do.Publicize();
			}

			isolated1.Do.Patch();
			isolated1.Do.Write();
		}

		using Sandbox<RustHarmony> isolated2 = new Sandbox<RustHarmony>();
		{
			isolated2.Do.Patch();
			isolated2.Do.Write();
		}

		using Sandbox<FacepunchConsole> isolated3 = new Sandbox<FacepunchConsole>();
		{
			isolated3.Do.Patch();
			isolated3.Do.Write();
		}

		using Sandbox<FacepunchNetwork> isolated4 = new Sandbox<FacepunchNetwork>();
		{
			if(!isolated4.Do.IsPublic("Networkable", "sv"))
			{
				isolated4.Do.Publicize();
			}

			isolated4.Do.Write();
		}

		using Sandbox<RustClansLocal> isolated5 = new Sandbox<RustClansLocal>();
		{
			if (!isolated5.Do.IsPublic("LocalClanDatabase"))
			{
				isolated5.Do.Publicize();
			}

			isolated5.Do.Write();
		}

		using Sandbox<FacepunchNexus> isolated6 = new Sandbox<FacepunchNexus>();
		{
			if (!isolated6.Do.IsPublic("Util"))
			{
				isolated6.Do.Publicize();
			}

			isolated6.Do.Write();
		}

		foreach (string file in Preload)
		{
			try
			{
				Assembly harmony = Assembly.LoadFile(file);
				Logger.Log($"Loaded {harmony.GetName().Name} {harmony.GetName().Version} into current AppDomain");
			}
			catch (System.Exception e)
			{
				Logger.Log($"Unable to preload '{file}' ({e?.Message})");
			}
		}
	}
}
