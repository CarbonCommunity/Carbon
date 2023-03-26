using System;
using API.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Plugins;

public abstract class CarbonPlugin : ICarbonPlugin
{
	public ILogger Logger { get; internal set; }

	public abstract void Initialize(string identifier);
	public abstract void OnLoaded(EventArgs args);
	public abstract void OnUnloaded(EventArgs args);
}