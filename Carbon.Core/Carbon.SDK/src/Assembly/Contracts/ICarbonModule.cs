using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public interface ICarbonModule : ICarbonAddon
{
	public void OnEnable(EventArgs args);
	public void OnDisable(EventArgs args);
}
