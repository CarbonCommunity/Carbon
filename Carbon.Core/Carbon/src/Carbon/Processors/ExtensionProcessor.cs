using System;
using System.Linq;
using Carbon.Base;
using Carbon.Common;
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
		foreach (var type in AccessToolsEx.AllTypes().Where(x => x.GetInterfaces().Contains(typeof(IExtension))))
		{
			var ext = Activator.CreateInstance(type) as IExtension;
			if (init) ext.OnInit(); else ext.OnUnload();

			Logger.Log($"{(init ? "Installed" : "Uninstalled")} Carbon extension '{type.FullName}'");
			count++;
		}
	}
}
