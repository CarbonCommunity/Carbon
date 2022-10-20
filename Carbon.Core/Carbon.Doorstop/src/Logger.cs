///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;
using Carbon.Utility;

namespace Carbon
{
	internal class Logger
	{
		private static string logFile
			= Path.Combine(Context.Base, "__doorstop.log");

		internal enum Severity
		{
			Error, Warning, Notice, None
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

				case Severity.None:
					Console.WriteLine(message);
					return;

				default:
					throw new Exception($"Severity {severity} not implemented.");
			}

			Console.WriteLine(formatted);
			File.AppendAllText(logFile, $"{timestamp}{formatted}" + Environment.NewLine);
		}

		internal static void None(object message)
			=> Write(Logger.Severity.None, message);

		internal static void Log(object message)
			=> Write(Logger.Severity.Notice, message);

		internal static void Warn(object message)
			=> Write(Logger.Severity.Warning, message);

		internal static void Error(object message, Exception ex = null)
			=> Write(Logger.Severity.Error, message, ex);
	}
}
