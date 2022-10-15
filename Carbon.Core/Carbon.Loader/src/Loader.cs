///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using Carbon.Patterns;
using Carbon.Utility;

namespace Carbon;

internal sealed class Loader : Singleton<Loader>, IDisposable
{
	static Loader() { }

	private readonly string Identifier = Guid.NewGuid().ToString();

	internal HarmonyLib.Harmony Harmony;

	internal Loader()
	{
		Logger.Warn($"Using '{Identifier}' as the Harmony namespace");
		Harmony = new HarmonyLib.Harmony($"{Identifier}");
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