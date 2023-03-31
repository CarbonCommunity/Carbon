/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Events;

public enum CarbonEvent
{
	// Startup events
	StartupShared, StartupSharedComplete,

	// Carbon related
	CarbonStartup, CarbonStartupComplete, CarbonShutdown, CarbonShutdownFailed,

	// Hook related
	HooksInstalled, HookValidatorRefreshed,

	// Plugin related
	AllPluginsLoaded, PluginLoaded, PluginUnloaded,

	// Things that look like hooks
	OnServerInitialized, OnServerSave
}
