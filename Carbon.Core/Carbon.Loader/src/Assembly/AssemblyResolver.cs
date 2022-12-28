using System;
using System.Reflection;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Utility;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.ASM;

internal sealed class AssemblyResolver : Singleton<AssemblyResolver>, IDisposable
{
	private ResolverEx _resolver;

	static AssemblyResolver() { }

	internal AssemblyResolver()
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
#if DEBUG
		Logger.Debug($"LoadAssembly: {file}");
#endif
		return _resolver.ResolveAssembly(this, args: new ResolveEventArgs(file));
	}

	internal byte[] ReadAssembly(string file)
	{
#if DEBUG
		Logger.Debug($"ReadAssembly: {file}");
#endif
		return _resolver.ResolveAssembly(file, "internal").Bytes;
	}
}
