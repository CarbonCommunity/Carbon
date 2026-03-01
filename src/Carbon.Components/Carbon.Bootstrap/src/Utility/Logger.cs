using System;
using System.IO;
using System.Reflection;
using Carbon.Extensions;

namespace Utility;

internal sealed class Logger
{
	private static readonly string logFile
		= Path.Combine(Context.CarbonLogs, $"{Assembly.GetExecutingAssembly().GetName().Name}.log");

	internal enum Severity
	{
		Error, Warning, Notice, Debug, None
	}

	public static object Lock = new();

	static Logger()
	{
		if (!Directory.Exists(Context.CarbonLogs))
			Directory.CreateDirectory(Context.CarbonLogs);
	}

	internal static void Write(Severity severity, object message, Exception ex = null)
	{
		string formatted = severity switch
		{
			Severity.None => $"{message}",
			Severity.Notice => $"{message}",
			Severity.Warning => $"[w] {message}",
			Severity.Error => $"[e] {message}{(ex == null ? string.Empty : $" ({ex.Message})\n{ex.GetFullStackTrace(false)}")}",
			Severity.Debug => $"[d] {message}",
			_ => throw new Exception($"Severity {severity} not implemented.")
		};

		lock (Lock)
		{
#if DEBUG
			if (ex is not null)
			{
				formatted += $" ({ex?.Message})\n{ex?.StackTrace}";
			}

			var color = Console.ForegroundColor;
			switch (severity)
			{
				case Severity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case Severity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
			}

			if (severity != Severity.Debug)
			{
				Console.WriteLine(formatted);
				Console.ForegroundColor = color;
			}
#endif
			File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {formatted}" + Environment.NewLine);
		}
	}

	internal static void None(object message) => Write(Logger.Severity.None, message);

	internal static void Log(object message) => Write(Logger.Severity.Notice, message);

	internal static void Warn(object message) => Write(Logger.Severity.Warning, message);

	internal static void Error(object message, Exception ex = null) => Write(Logger.Severity.Error, message, ex);
}
