namespace Carbon.Hooks;

/// <summary>
/// Base class of every Carbon hook patch. Nest your Harmony prefix/postfix/transpiler in a subclass
/// and describe it with <see cref="HookAttribute.Patch"/>.
/// </summary>
public class Patch
{
	protected static EventManager Events => Services.Events;
}
