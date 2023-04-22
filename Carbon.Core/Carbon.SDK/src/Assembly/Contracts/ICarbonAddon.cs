using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface ICarbonAddon
{
	// We need to implement the same thing we have for plugins
	// something like a UID, name, description etc
	//public string Name { get; }

	public void Awake(EventArgs args);
	public void OnLoaded(EventArgs args);
	public void OnUnloaded(EventArgs args);
}
