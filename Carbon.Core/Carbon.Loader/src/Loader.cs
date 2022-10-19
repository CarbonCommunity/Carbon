///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Common;
using Carbon.Utility;

namespace Carbon;

internal sealed class Loader : Singleton<Loader>, IDisposable
{
	static Loader() { }

	private readonly string Identifier;

	internal HarmonyLib.Harmony Harmony;

	private UnityEngine.GameObject gameObject;

	private static Dictionary<string, Assembly> loadedAssembly;

	internal Loader()
	{
		Identifier = Guid.NewGuid().ToString();
		Logger.Warn($"Using '{Identifier}' as runtime namespace");

		gameObject = new UnityEngine.GameObject(Identifier);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);

		Harmony = new HarmonyLib.Harmony(Identifier);
		loadedAssembly = new Dictionary<string, Assembly>();
	}

	public void Initialize()
	{
		AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
		//gameObject.AddComponent<HarmonyWatcher>();
	}

	internal static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
	{
		var asmName = new AssemblyName(args.Name);
		var asmPath = Path.Combine(Context.Directory.CarbonLib, $"{asmName.Name}.dll");

		if (loadedAssembly.TryGetValue(asmName.Name, out Assembly cached))
		{
			Logger.Log($"Resolved: {asmName.Name} [{args.RequestingAssembly.GetName().Name}] from cache");
			return cached;
		}
		else if (File.Exists(asmPath))
		{
			byte[] raw = File.ReadAllBytes(asmPath);
			Assembly asm = Assembly.Load(raw);

			Logger.Log($"Resolved: {asmName.Name} [{args.RequestingAssembly.GetName().Name}] from disk");
			loadedAssembly.Add(asmName.Name, asm);
			return asm;
		}

		Logger.Warn($"Unable to resolve ref: {asmName.Name} [{args.RequestingAssembly.GetName().Name}]");
		return null;
	}

	private bool IsDisposed = false;

	public void Dispose()
	{
		if (IsDisposed) return;

		try
		{
			Harmony.UnpatchAll(Harmony.Id);
			Logger.Log("Removed all Harmony patches");
		}
		catch (Exception e)
		{
			Logger.Error("Unable to remove all Harmony patches", e);
		}

		Harmony = default;
		IsDisposed = true;
	}
}
