/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Extensions;

public static class ExceptionEx
{
	public static string GetFullStackTrace(this Exception exception, bool mainMessage = true)
	{
		var fullStackTrace = mainMessage ? exception.ToString() : exception.StackTrace;
		var innerException = exception.InnerException;

		while (innerException != null)
		{
			fullStackTrace += "\n  Inner exception:\n  " + innerException.ToString();
			innerException = innerException.InnerException;
		}

		return fullStackTrace;
	}
}
