///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Reflection;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Components;
using Carbon.LoaderEx.Utility;

namespace Carbon.LoaderEx;

internal sealed class Program : Singleton<Program>, IDisposable
{
	internal static readonly string identifier;
	internal static readonly string assemblyName;

	internal HarmonyLib.Harmony Harmony;

	private UnityEngine.GameObject gameObject;

	static Program()
	{
		identifier = Guid.NewGuid().ToString();
		Logger.Warn($"Using '{identifier}' as runtime namespace");
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		Logger.Warn($"Facepunch baptized us as '{assemblyName}', látom.");
		AssemblyResolver.GetInstance().RegisterDomain(AppDomain.CurrentDomain);
	}

	internal Program()
	{
		Harmony = new HarmonyLib.Harmony(identifier);
		gameObject = new UnityEngine.GameObject(identifier);
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

	public void Dispose()
	{
		try
		{
			Harmony.UnpatchAll(identifier);
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
