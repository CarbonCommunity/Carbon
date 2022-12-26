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

	internal void Register(AppDomain domain)
	{
		_resolver.RegisterDomain(AppDomain.CurrentDomain);
	}

	public void Dispose()
	{
		_resolver.Dispose();
		_resolver = default;
	}

	internal Assembly LoadAssembly(string file, bool forced = false)
	{
		Logger.Debug($"LoadAssembly >>> {file}");
		return _resolver.ResolveAssembly(this, new ResolveEventArgs("Carbon"));
	}
}
