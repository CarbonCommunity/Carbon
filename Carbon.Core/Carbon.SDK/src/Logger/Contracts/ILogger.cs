using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Logger;

public interface ILogger
{
	/// <summary>
	/// Outputs the message to the game's console with user selected severity level.
	/// By default the severity will be 'NOTICE'.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="severity"></param>
	/// <param name="exception"></param>
	public void Console(string message, Severity severity = Severity.Notice, Exception exception = null);
}
