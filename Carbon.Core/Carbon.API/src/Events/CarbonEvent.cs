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
	// Bootstrap.SharedStartup
	StartupShared, StartupSharedComplete,

	// Carbon.Core
	CarbonStartup, CarbonStartupComplete, CarbonShutdown, CarbonShutdownFailed,

	// Carbon.Hooks
	HooksInstalled, HookValidatorRefreshed,

	// Carbon.Plugins
	OnPluginProcessFinished,


	// Hook like stuff..
	OnServerInitialized
}
