///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;

namespace Carbon
{
	public class Logger
	{
		private const string logFile = "__doorstop.log";

		internal enum Severity
		{
			Error, Warning, Notice, None
		}

		internal static void Write(Severity severity, object message, Exception ex = null)
		{
			string formatted = null;

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
					formatted = $"{message}";
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");
			}

			Console.WriteLine(formatted);
			File.AppendAllText(logFile, formatted + Environment.NewLine);
		}

		public static void None(object message)
			=> Write(Logger.Severity.None, message);

		public static void Log(object message)
			=> Write(Logger.Severity.Notice, message);

		public static void Warn(object message)
			=> Write(Logger.Severity.Warning, message);

		public static void Error(object message, Exception ex = null)
			=> Write(Logger.Severity.Error, message, ex);
	}
}
