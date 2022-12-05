using System;
using System.Reflection;
using System.Threading.Tasks;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Utility;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx;

internal sealed class Program : Singleton<Program>, IDisposable
{
	internal static readonly string identifier;
	internal static readonly string assemblyName;

	internal HarmonyLib.Harmony _harmonyInstance;
	private UnityEngine.GameObject _gameObject;


	internal HarmonyLib.Harmony Harmony
	{ get => _harmonyInstance; }

	internal DownloadManager Downloader
	{ get => _gameObject.GetComponent<DownloadManager>(); }

	static Program()
	{
		identifier = $"{Guid.NewGuid():N}";
		Logger.Warn($"Using '{identifier}' as runtime namespace");
		assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
		Logger.Warn($"Facepunch baptized us as '{assemblyName}', látom.");
		AssemblyResolver.GetInstance().RegisterDomain(AppDomain.CurrentDomain);
	}

	internal Program()
	{
		_harmonyInstance = new HarmonyLib.Harmony(identifier);
		_gameObject = new UnityEngine.GameObject(identifier);
		_gameObject.AddComponent<DownloadManager>();
		UnityEngine.Object.DontDestroyOnLoad(_gameObject);
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

		// keeping it disabled for now
		//AssemblyResolver.GetInstance().WarmupAssemblies();
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

		_harmonyInstance = default;
		_gameObject = default;
	}
}
