using System;
using System.Reflection;
using Carbon.Extensions;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Supervisor;

internal static class ASM
{
	private static Type _loader, _assemblyManager, _harmonyLoader;

	private static Func<object> _loaderInstance, _assemblyManagerInstance, _harmonyLoaderInstance;


	/// <summary>
	/// Gets the array of bytes of a loaded type from AppDomain.<br/>
	/// If the assembly is not found at the registered App Domain, an exception
	/// will be thrown.
	/// </summary>
	///
	/// <param name="name">Assembly name, short of full name</param>
	internal static Func<string, byte[]> ReadAssembly;

	/// <summary>
	/// Loads assembly into AppDomain by always reading it back from disk.<br/>
	/// The process will automatically detect the type of file being loaded
	/// and use the appropriate loading method to deal with it.
	/// </summary>
	///
	/// <param name="fileName">Assembly name with extension i.e. the file name</param>
	/// <param name="forced">Forces the assembly to be re-read from disk</param>
	internal static Func<string, Assembly> LoadModule;

	/// <summary>
	/// Calls the assembly OnUnloaded() and Dispose() methods to "unload".<br/>
	/// Due to the limitations of using only one AppDomain, the assembly will
	/// still be present on the loaded modules.
	/// </summary>
	///
	/// <param name="fileName">Assembly name with extension i.e. the file name</param>
	/// <param name="reload">Loads back the assembly</param>
	internal static Action<string, bool> UnloadModule;

	internal static Func<string, bool> IsLoaded;

	static ASM()
	{
		try
		{
			_loader = AccessToolsEx.TypeByName("Legacy.Loader") ?? null;
			if (_loader == null) throw new Exception("Loader is null");

			_harmonyLoader = AccessToolsEx.TypeByName("Legacy.Harmony.HarmonyLoaderEx") ?? null;
			if (_harmonyLoader == null) throw new Exception("HarmonyLoaderEx is null");

			_assemblyManager = AccessToolsEx.TypeByName("Legacy.ASM.AssemblyManager") ?? null;
			if (_assemblyManager == null) throw new Exception("AssemblyManager is null");


			_loaderInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_loader.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

			_assemblyManagerInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_assemblyManager.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

			_harmonyLoaderInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_harmonyLoader.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));


			IsLoaded = (Func<string, bool>)Delegate.CreateDelegate(typeof(Func<string, bool>), _harmonyLoaderInstance(), "IsLoaded");
			LoadModule = (Func<string, Assembly>)Delegate.CreateDelegate(typeof(Func<string, Assembly>), _harmonyLoaderInstance(), "Load");
			UnloadModule = (Action<string, bool>)Delegate.CreateDelegate(typeof(Action<string, bool>), _harmonyLoaderInstance(), "Unload");
			ReadAssembly = (Func<string, byte[]>)Delegate.CreateDelegate(typeof(Func<string, byte[]>), _assemblyManagerInstance(), "ReadAssembly");
		}
		catch (System.Exception e)
		{
			Logger.Error($"Supervisor late bind error, this is a bug", e);
		}
	}
}
