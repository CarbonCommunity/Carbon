using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Events;

public enum CarbonEvent
{
	// Debug
	TestEventOne,

	// Bootstrap.SharedStartup
	StartupShared, StartupSharedComplete,

	// Carbon.Core
	CarbonStartup, CarbonStartupComplete, CarbonShutdown, CarbonShutdownComplete, CarbonShutdownFailed,

	// Carbon.Hooks
	HookManagerHooksReady,

	// Carbon.Plugins
	CarbonPluginsReady
}
