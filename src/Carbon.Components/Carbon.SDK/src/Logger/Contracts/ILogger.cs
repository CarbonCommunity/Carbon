using System;

namespace API.Logger;

public interface ILogger
{
	/// <summary>
	/// Outputs the message to the game's console with user selected severity level.
	/// By default the severity will be 'NOTICE'.
	/// </summary>
	public void Console(string message, Severity severity = Severity.Notice, Exception exception = null);
}
