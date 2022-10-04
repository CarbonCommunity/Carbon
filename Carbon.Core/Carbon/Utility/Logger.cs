///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Carbon.Core;
using Facepunch;
using UnityEngine;

namespace Carbon
{
	public class Logger
	{
		private Logger() { }

		public enum Severity
		{
			Critical,
			Error,
			Warning,
			Notice,
			Informational,
			Debug
		}

		internal static Severity _consoleSeverityThreshold = Severity.Notice;
		internal static void Write(Severity severity, object message, Exception ex = null)
		{
			if (severity > _consoleSeverityThreshold) return;

			switch (severity)
			{
				case Severity.Critical:
				case Severity.Error:
					var demystifiedException = ex?.Demystify() ?? ex;
					if (demystifiedException != null) UnityEngine.Debug.LogError($"{message} ({demystifiedException?.Message})\n{demystifiedException?.StackTrace}");
					else UnityEngine.Debug.LogError(message); break;

				case Severity.Warning:
					UnityEngine.Debug.LogWarning(message);
					break;

				case Severity.Notice:
				case Severity.Informational:
				case Severity.Debug:
					UnityEngine.Debug.Log(message);
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");
			}
		}

		public static void Debug(object message, int verbosityLevel = 1, LogType log = LogType.Log) => Debug(null, message, verbosityLevel, log);
		public static void Debug(object header, object message, int verbosityLevel = 1, LogType log = LogType.Log)
		{
			if (CarbonCore.Instance.Config.LogVerbosity <= -1 ||
				CarbonCore.Instance.Config.LogVerbosity <= verbosityLevel) return;

			if (header != null) header = $".{header}";

			switch (log)
			{
				case LogType.Log:
					Log($"[CRBN{header}] {message}");
					break;

				case LogType.Warning:
					Warn($"[CRBN{header}] {message}");
					break;

				case LogType.Error:
					Error($"[CRBN{header}] {message}");
					break;
			}
		}

		public static void Log(object message) => Carbon.Logger.Write(Logger.Severity.Notice, message);
		public static void Log(object header, object message) => Log($"[{header}] {message}");
		public static void Format(string format, params object[] args) => Log(string.Format(format, args));

#if DEBUG
		public static void Warn(object message,
			[CallerLineNumber] int line = 0,
			[CallerFilePath] string path = null,
			[CallerMemberName] string method = null)
		{
			if (CarbonCore.Instance.Config.LogVerbosity > 0) message = $"{message}\n (file: {Path.GetFileName(path)}, method: {method}, line: {line}]\n";
			Carbon.Logger.Write(Logger.Severity.Warning, message);
		}
#else
		public static void Warn(object message) => Carbon.Logger.Write(Logger.Severity.Warning, message);
#endif
		public static void Warn(object header, object message) => Warn($"[{header}] {message}");

		public static void WarnFormat(string format, params object[] args) => Warn(string.Format(format, args));

#if DEBUG
		public static void Error(object message, Exception ex = null,
			[CallerLineNumber] int line = 0,
			[CallerFilePath] string path = null,
			[CallerMemberName] string method = null)
		{
			if (CarbonCore.Instance.Config.LogVerbosity > 0) message = $"{message}\n (file: {Path.GetFileName(path)}, method: {method}, line: {line}]\n";
			Carbon.Logger.Write(Logger.Severity.Error, message, ex);
		}
#else
		public static void Error(object message, Exception ex = null) => Carbon.Logger.Write(Logger.Severity.Error, message, ex);
#endif
		public static void Error(object header, object message, Exception ex = null) => Error($"[{header}] {message}", ex);

		public static void ErrorFormat(string format, Exception ex = null, params object[] args) => Error(string.Format(format, args), ex);
	}
}
