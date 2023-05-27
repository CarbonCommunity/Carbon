using System;
using API.Assembly;
using API.Contracts;
using API.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Plugins;

public interface ICarbonPlugin : ICarbonAddon
{
	public ILogger Logger { get; }

	public void OnEnable(EventArgs args);
	public void OnDisable(EventArgs args);
}
