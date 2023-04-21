using System;
using API.Contracts;
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
	public ICarbonInterface Interface { get; internal set; }

	public abstract void OnLoaded(EventArgs args);
	public abstract void OnUnloaded(EventArgs args);

	public abstract void Awake(EventArgs args);
	public abstract void OnEnable(EventArgs args);
	public abstract void OnDisable(EventArgs args);
}
