using System;
using System.IO;
using System.Reflection;
using Carbon.Startup.Core;

namespace Doorstop.Utility;

public sealed class Logger
{
	private static string logFile
		= Path.Combine(Defines.GetLogsFolder(), $"{Assembly.GetExecutingAssembly().GetName().Name}.log");

	internal enum Severity
	{
		Error, Warning, Notice, Debug, None
	}

	public static object locker = new();

	static Logger()
	{
		if (!Directory.Exists(Defines.GetLogsFolder()))
		{
			Directory.CreateDirectory(Defines.GetLogsFolder());
		}
	}

	internal static void Write(Severity severity, object message, Exception ex = null)
	{
		string formatted = severity switch
		{
			Severity.None => $"{message}",
			Severity.Notice => $"{message}",
			Severity.Warning => $"[w] {message}",
			Severity.Error => $"[e] {message}",
			Severity.Debug => $"[d] {message}",
			_ => throw new Exception($"Severity {severity} not implemented.")
		};

		lock (locker)
		{
			if (ex is not null)
			{
				formatted += $" ({ex?.Message})\n{ex?.StackTrace}";
			}

			switch (severity)
			{
				case Severity.Error:
				case Severity.Notice:
					Console.WriteLine(formatted);
					break;
			}

			File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {formatted}" + Environment.NewLine);
		}
	}

	public static void None(object message) => Write(Logger.Severity.None, message);
	public static void Debug(object message) => Write(Logger.Severity.Debug, message);
	public static void Log(object message) => Write(Logger.Severity.Notice, message);
	public static void Warn(object message) => Write(Logger.Severity.Warning, message);
	public static void Error(object message, Exception ex = null) => Write(Logger.Severity.Error, message, ex);
}
