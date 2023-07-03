using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

internal sealed class Logger
{
	private static string logFile
		= Path.Combine(Context.CarbonLogs, $"{Assembly.GetExecutingAssembly().GetName().Name}.log");

	internal enum Severity
	{
		Error, Warning, Notice, Debug, None
	}

	public static List<int> Lock = new();

	static Logger()
	{
		if (!Directory.Exists(Context.CarbonLogs))
			Directory.CreateDirectory(Context.CarbonLogs);
		//else if (File.Exists(logFile)) File.Delete(logFile);
	}

	internal static void Write(Severity severity, object message, Exception ex = null)
	{
		string formatted = severity switch
		{
			Severity.None => $"{message}",
			Severity.Notice => $"[i] {message}",
			Severity.Warning => $"[w] {message}",
			Severity.Error => $"[e] {message}",
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

			if (severity != Severity.Debug)
			{
				Console.WriteLine(formatted);
			}
#endif
			File.AppendAllText(logFile,
				$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {formatted}" + Environment.NewLine);
		}
	}

	internal static void None(object message)
		=> Write(Logger.Severity.None, message);

	internal static void Debug(object message)
		=> Write(Logger.Severity.Debug, message);

	internal static void Log(object message)
		=> Write(Logger.Severity.Notice, message);

	internal static void Warn(object message)
		=> Write(Logger.Severity.Warning, message);

	internal static void Error(object message, Exception ex = null)
		=> Write(Logger.Severity.Error, message, ex);
}
