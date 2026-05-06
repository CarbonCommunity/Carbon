using System;

namespace API.Hooks;

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
