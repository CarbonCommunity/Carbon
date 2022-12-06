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

internal static class Resolver
{
	private static Type _assemblyResolver;
	private static MethodInfo _getInstance;
	private static MethodInfo _getAssembly;
	private static MethodInfo _getAssemblyBytes;

	private static Func<object> GetInstance;

	static Resolver()
	{
		try
		{
			_assemblyResolver = AccessTools.TypeByName("Carbon.LoaderEx.AssemblyResolver") ?? null;
			if (_assemblyResolver == null) throw new Exception("AssemblyResolver is null");

			_getInstance = _assemblyResolver.GetMethod("GetInstance",
				BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) ?? null;
			if (_getInstance == null) throw new Exception("AssemblyResolver instance is null");

			GetInstance = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>),
				_assemblyResolver.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));

			_getAssembly = _assemblyResolver.GetMethod("GetAssembly",
				BindingFlags.Public | BindingFlags.Instance) ?? null;

			_getAssemblyBytes = _assemblyResolver.GetMethod("GetAssemblyBytes",
				BindingFlags.Public | BindingFlags.Instance) ?? null;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Supervisor late bind error, this is a bug", e);
		}
	}

	internal static Assembly GetAssembly(string name)
		=> (Assembly)_getAssembly.Invoke(GetInstance(), new object[] { name });

	internal static byte[] GetAssemblyBytes(string name)
		=> (byte[])_getAssemblyBytes.Invoke(GetInstance(), new object[] { name });
}