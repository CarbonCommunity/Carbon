using System;

namespace Carbon.Hooks;

/// <summary>
/// How a hook gets patched. No Static/Patch flag means it's only patched while something subscribes to it.
/// </summary>
[Flags]
public enum HookFlags
{
	None = 0,
	Static = 1,
	Patch = 2,
	Hidden = 4,
	IgnoreChecksum = 8,
	MetadataOnly = 16
}
