using System;

namespace Carbon.Commands;

[Flags]
public enum CommandFlags
{
	None,
	Hidden,
	Protected
}
