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

internal static class Loader
{
	private static Type _harmonyLoader;
	private static MethodInfo _getInstance;
	private static MethodInfo _load;
	private static MethodInfo _unload;

	private static Func<object> GetInstance;

	static Loader()
	{
		try
		{
			_harmonyLoader = AccessTools.TypeByName("Carbon.LoaderEx.HarmonyLoaderEx") ?? null;
			if (_harmonyLoader == null) throw new Exception("HarmonyLoaderEx is null");

			_getInstance = _harmonyLoader.GetMethod("GetInstance",
				BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ?? null;
			if (_getInstance == null) throw new Exception("HarmonyLoaderEx instance is null");

			GetInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_harmonyLoader.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

			_load = _harmonyLoader.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Instance);
			if (_load == null) throw new Exception("HarmonyLoaderEx load is null");

			_unload = _harmonyLoader.GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Instance);
			if (_unload == null) throw new Exception("HarmonyLoaderEx unload is null");
		}
		catch (System.Exception e)
		{
			Logger.Error($"Supervisor late bind error, this is a bug", e);
		}
	}

	internal static void Load(string fileNameWithExtension)
		=> _load.Invoke(GetInstance(), new object[] { fileNameWithExtension });

	internal static void Unload(string fileNameWithExtension, bool reload = false)
		=> _unload.Invoke(GetInstance(), new object[] { fileNameWithExtension, reload });

	internal static void Update(object os, object release, Action<bool> callback = null)
	{
		try
		{
			Type type = AccessTools.TypeByName("Carbon.LoaderEx.Common.Updater") ?? null;
			MethodInfo method = type.GetMethod("UpdateCarbon", BindingFlags.NonPublic | BindingFlags.Static) ?? null;
			method.Invoke(null, new object[] { os, release, callback });
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while updating Carbon", e);
		}
	}
}