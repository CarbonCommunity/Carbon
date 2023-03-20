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
	private static readonly string logFile
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
		lock (Lock)
		{
			string formatted = null;

			switch (severity)
			{
				case Severity.Error:
					formatted = $"[e] {message}";
					formatted += (ex != null) ? $"({ex?.Message})\n{ex?.StackTrace}" : null;
					break;

				case Severity.Warning:
					formatted = $"[w] {message}";
					break;

				case Severity.Notice:
					formatted = $"[i] {message}";
					break;

				case Severity.Debug:
					formatted = $"[d] {message}";
					break;

				case Severity.None:
					formatted = $"{message}";
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");
			}

#if DEBUG_VERBOSE
			UnityEngine.Debug.Log(formatted);
#endif
			System.IO.File.AppendAllText(logFile,
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
