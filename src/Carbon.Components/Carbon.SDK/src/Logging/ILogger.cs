using System;

namespace Carbon.Logging;

/// <summary>
/// Logging abstraction for components that can't reference Carbon.Common.
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Outputs the message to the game's console with user selected severity level.
	/// By default the severity will be 'NOTICE'.
	/// </summary>
	public void Console(string message, Severity severity = Severity.Notice, Exception exception = null);
}
