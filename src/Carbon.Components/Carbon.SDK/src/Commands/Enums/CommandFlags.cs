using System;

namespace API.Commands;

[Flags]
public enum CommandFlags
{
	None,
	Hidden,
	Protected
}
