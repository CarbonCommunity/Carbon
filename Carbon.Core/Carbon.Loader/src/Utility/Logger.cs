using System;
using System.IO;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Utility;

internal sealed class Logger
{
	private static string logFile
		= Path.Combine(Context.Directories.CarbonLogs, "Carbon.Loader.log");

	internal enum Severity
	{
		Error, Warning, Notice, Debug, RCon, None
	}

	static Logger()
	{
		if (!Directory.Exists(Context.Directories.CarbonLogs))
			Directory.CreateDirectory(Context.Directories.CarbonLogs);
		if (File.Exists(logFile)) File.Delete(logFile);
	}

	internal static void Write(Severity severity, object message, Exception ex = null)
	{
		string formatted = null;
		string timestamp = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ";

		switch (severity)
		{
			case Severity.Error:
				if (ex != null)
					formatted = $"[e] {message} ({ex?.Message})\n{ex?.StackTrace}";
				else
					formatted = $"[e] {message}";
				break;

			case Severity.Warning:
				formatted = $"[w] {message}";
				break;

			case Severity.Notice:
				formatted = $"[i] {message}";
				break;

			case Severity.Debug:
				formatted = $"[d] {message}";
				System.IO.File.AppendAllText(logFile, $"{timestamp}{formatted}" + Environment.NewLine);
				return;

			case Severity.RCon:
				UnityEngine.Debug.Log($"[CRBN] {message}");
				System.IO.File.AppendAllText(logFile, $"{timestamp}{formatted}" + Environment.NewLine);
				return;

			case Severity.None:
				Console.WriteLine(message);
				return;

			default:
				throw new Exception($"Severity {severity} not implemented.");
		}

		Console.WriteLine(formatted);
		System.IO.File.AppendAllText(logFile, $"{timestamp}{formatted}" + Environment.NewLine);
	}

	internal static void None(object message)
		=> Write(Logger.Severity.None, message);

	internal static void RCon(object message)
		=> Write(Logger.Severity.RCon, message);

	internal static void Debug(object message)
		=> Write(Logger.Severity.Debug, message);

	internal static void Log(object message)
		=> Write(Logger.Severity.Notice, message);

	internal static void Warn(object message)
		=> Write(Logger.Severity.Warning, message);

	internal static void Error(object message, Exception ex = null)
		=> Write(Logger.Severity.Error, message, ex);
}
