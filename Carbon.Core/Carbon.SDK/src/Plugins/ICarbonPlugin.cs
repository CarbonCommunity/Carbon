using System;
using API.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Plugins;

public interface ICarbonPlugin
{
	public ILogger Logger { get; }

	public void Initialize(string identifier);
	public void OnLoaded(EventArgs args);
	public void OnUnloaded(EventArgs args);
}