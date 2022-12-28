using System;
using System.Reflection;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Supervisor;

internal static class ASM
{
	private static Type _assemblyResolver, _harmonyLoader;

	private static Func<object> GetResolverInstance, GetLoaderInstance;


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

	static ASM()
	{
		try
		{
			_harmonyLoader = AccessTools.TypeByName("Carbon.LoaderEx.Harmony.HarmonyLoaderEx") ?? null;
			if (_harmonyLoader == null) throw new Exception("HarmonyLoaderEx is null");

			_assemblyResolver = AccessTools.TypeByName("Carbon.LoaderEx.ASM.AssemblyResolver") ?? null;
			if (_assemblyResolver == null) throw new Exception("AssemblyResolver is null");


			GetResolverInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_assemblyResolver.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

			GetLoaderInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_harmonyLoader.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));



			ReadAssembly = (Func<string, byte[]>)Delegate
				.CreateDelegate(typeof(Func<string, byte[]>), GetResolverInstance(), "ReadAssembly");

			LoadModule = (Func<string, Assembly>)Delegate
				.CreateDelegate(typeof(Func<string, Assembly>), GetLoaderInstance(), "Load");

			UnloadModule = (Action<string, bool>)Delegate
				.CreateDelegate(typeof(Action<string, bool>), GetLoaderInstance(), "Unload");
		}
		catch (System.Exception e)
		{
			Logger.Error($"Supervisor late bind error, this is a bug", e);
		}
	}

	internal static void Update(object os, object release, Action<bool> callback = null)
		=> throw new NotImplementedException();
}