///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Components;
using Carbon.LoaderEx.Utility;

namespace Carbon.LoaderEx;

public sealed class Program : Singleton<Program>, IDisposable
{
	static Program() { }

	private readonly string Identifier;

	internal HarmonyLib.Harmony Harmony;

	private UnityEngine.GameObject gameObject;

	internal Program()
	{
		Identifier = Guid.NewGuid().ToString();
		Logger.Warn($"Using '{Identifier}' as runtime namespace");
		AssemblyResolver.GetInstance().RegisterDomain(AppDomain.CurrentDomain);

		Harmony = new HarmonyLib.Harmony(Identifier);
		gameObject = new UnityEngine.GameObject(Identifier);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	internal void Initialize()
	{
		Logger.None(
			@"                                               " + Environment.NewLine +
			@"  ______ _______ ______ ______ _______ _______ " + Environment.NewLine +
			@" |      |   _   |   __ \   __ \       |    |  |" + Environment.NewLine +
			@" |   ---|       |      <   __ <   -   |       |" + Environment.NewLine +
			@" |______|___|___|___|__|______/_______|__|____|" + Environment.NewLine +
			@"                         discord.gg/eXPcNKK4yd " + Environment.NewLine +
			@"                                               " + Environment.NewLine
		);
	}

	// TODO I'd like to keep the loader class internal
	// maybe this need to go to a "more public" type
	public void Restart()
	{
		Components.HarmonyLoader.GetInstance().Unload("Carbon.dll");
		Components.HarmonyLoader.GetInstance().Load("Carbon.dll");
	}

	public void Dispose()
	{
		try
		{
			Harmony.UnpatchAll(Identifier);
			Logger.Log("Removed all Harmony patches");
		}
		catch (Exception e)
		{
			Logger.Error("Unable to remove all Harmony patches", e);
		}

		Harmony = default;
		gameObject = default;
	}
}
