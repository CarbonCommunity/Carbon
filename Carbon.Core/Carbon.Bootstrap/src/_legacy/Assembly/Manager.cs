// #define DEBUG_VERBOSE

using System;
using System.Reflection;
using Contracts;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Legacy.ASM;

internal sealed class AssemblyManager : Singleton<AssemblyManager>, IDisposable
{
	private ResolverEx _resolver;

	static AssemblyManager() { }

	internal AssemblyManager()
	{
		_resolver = new ResolverEx();
	}

	public void Dispose()
	{
		_resolver.Dispose();
		_resolver = default;
	}

	internal void Register(AppDomain domain)
	{
		_resolver.RegisterDomain(AppDomain.CurrentDomain);
	}

	internal Assembly LoadAssembly(string file)
	{
#if DEBUG_VERBOSE
		Logger.Debug($"LoadAssembly: {file}");
#endif
		return _resolver.ResolveAssembly(this, args: new ResolveEventArgs(file));
	}

	internal byte[] ReadAssembly(string file)
	{
#if DEBUG_VERBOSE
		Logger.Debug($"ReadAssembly: {file}");
#endif
		return _resolver.ResolveAssembly(file, "AssemblyManager").Bytes;
	}

	internal bool RemoveCache(string file)
	{
#if DEBUG_VERBOSE
		Logger.Debug($"RemoveCache: {file}");
#endif
		return _resolver.RemoveCache(file);
	}
}
