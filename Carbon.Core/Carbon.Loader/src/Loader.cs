///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using Carbon.Interfaces;
using Carbon.Patterns;
using Carbon.Utility;

namespace Carbon;

internal sealed class Loader : Singleton<Loader>, IBase, IDisposable
{
	static Loader() { }

	internal HarmonyLib.Harmony Harmony;

	private UnityEngine.GameObject gameObject;

	private readonly string Identifier = Guid.NewGuid().ToString();

	internal Loader()
	{
		Logger.Warn($"Using '{Identifier}' as runtime namespace");

		Harmony = new HarmonyLib.Harmony(Identifier);

		gameObject = new UnityEngine.GameObject(Identifier);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public void Initialize()
	{
		var s = gameObject.AddComponent<HarmonyWatcher>();
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
