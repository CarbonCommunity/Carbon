using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.Base;
using Carbon.Contracts;
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
		foreach(var type in AccessToolsEx.AllTypes().Where(x => x.GetInterface("Carbon.Core.IExtension") != null))
		{
			Logger.Log($"{(init ? "Installed" : "Uninstalled")} Carbon extension '{type.FullName}'");
		}
	}
}
