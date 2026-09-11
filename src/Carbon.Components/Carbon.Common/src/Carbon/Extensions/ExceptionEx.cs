namespace Carbon.Extensions;

public static class ExceptionEx
{
	public static string GetFullStackTrace(this Exception exception, bool mainMessage = true)
	{
		var fullStackTrace = mainMessage ? exception.ToString() : exception.StackTrace;
		var innerException = exception.InnerException;

		while (innerException != null)
		{
			fullStackTrace += "\n  Inner exception:\n  " + innerException;
			innerException = innerException.InnerException;
		}

		return fullStackTrace;
	}
}
