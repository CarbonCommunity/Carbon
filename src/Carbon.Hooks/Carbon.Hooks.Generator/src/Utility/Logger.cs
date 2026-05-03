namespace Carbon.Utility;

internal static class Logger
{
	internal static void None(object message)
	{
		Console.WriteLine($"{message}");
	}

	internal static void Information(object message)
	{
		ConsoleWrite($"[i] {message}");
	}

	internal static void Warning(object message)
	{
		ConsoleWrite($"[w] {message}");
	}

	internal static void Error(object message)
	{
		ConsoleWrite($"[e] {message}");
	}

	private static void ConsoleWrite(object message)
	{
		Console.WriteLine($"{message}");
	}

	internal static string GetAssemblyName()
	{
		return typeof(Carbon.Program).Assembly.GetName().Name ?? "Carbon.Hookah";
	}
}
