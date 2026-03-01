namespace API.Events;

public enum CarbonEvent
{
	// Startup events
	StartupShared, StartupSharedComplete,
	FileSystemWarmup, FileSystemWarmupComplete,

	// Carbon related
	CarbonStartup, CarbonStartupComplete, CarbonShutdown, CarbonShutdownComplete, CarbonShutdownFailed,

	// Hook related
	HooksInstalled, HookValidatorRefreshed, HookFetchStart, HookFetchEnd, HooksPatchedFullFrame,

	// Component related
	ComponentLoaded, ComponentUnloaded, ComponentLoadFailed, ComponentUnloadFailed,

	// Extension related
	ExtensionLoaded, ExtensionUnloaded, ExtensionLoadFailed, ExtensionUnloadFailed,

	// Harmony related
	HarmonyLoaded, HarmonyLoadFailed,

	// Module related
	ModuleLoaded, ModuleUnloaded, ModuleLoadFailed, ModuleUnloadFailed,

	// Plugin related
	AllPluginsLoaded, AllPluginsInitialized, PluginPreload, PluginLoaded, PluginUnloaded,

	// Things that look like hooks
	OnServerInitialized, OnServerSave
}
