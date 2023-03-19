using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

[Serializable]
public sealed class Sandbox<T> : IDisposable where T : MarshalByRefObject
{
	private AppDomain _domain;
	private readonly T _proxy;
	private readonly string _identifier;

	public T GetProxy
	{ get => _proxy; }

	public Sandbox()
	{
		_identifier = $"sandbox_{Guid.NewGuid():N}";
		AppDomainSetup domaininfo = new AppDomainSetup
		{
			// this is still not perfect but it let's run with it for now.. ideally
			// the sandbox should be able to resolve their load requests using the 
			// domain assembly resolver event
			ApplicationBase = Context.CarbonManaged
		};

		_domain = AppDomain.CreateDomain(_identifier, null, domaininfo);
		Logger.Log($"Created a new AppDomain '{_identifier}'");

		Type type = typeof(T);
		_proxy = (T)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
		Logger.Log($"Created a new AppDomain '{_identifier}' with a proxy to '{type.Name}'");
	}

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				if (_domain != null)
				{
					Logger.Log($"Unloading AppDomain '{_identifier}'");
					AppDomain.Unload(_domain);
					_domain = default;
				}
			}
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
