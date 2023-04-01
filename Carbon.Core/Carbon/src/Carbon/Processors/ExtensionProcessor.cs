using System;
using System.Linq;
using API.Contracts;
using Carbon.Base;
using Carbon.Extensions;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

public class ExtensionProcessor : BaseProcessor
{
	public override string Name => "Extension Processor";

	public override void Start()
	{
		_propagateCall(true);
	}
	public override void OnDestroy() { }
	public override void Dispose()
	{
		_propagateCall(false);
	}

	internal void _propagateCall(bool init)
	{
		var count = 0;
		foreach (var type in AccessToolsEx.AllTypes().Where(x => x.GetInterfaces().Contains(typeof(ICarbonExtension))))
		{
			var ext = Activator.CreateInstance(type) as ICarbonExtension;
			try
			{
				if (init) ext.OnLoaded(EventArgs.Empty);
				else ext.OnUnloaded(EventArgs.Empty);

				Logger.Log($" {(init ? "Installed" : "Uninstalled")} Carbon extension '{type.FullName}'");
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed {(init ? "loading" : "unloading")} Carbon extension '{type.FullName}'", ex);
			}

			count++;
		}
	}
}
