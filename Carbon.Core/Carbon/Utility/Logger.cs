///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Carbon.Patterns;
using UnityEngine;

namespace Carbon
{
	public class Logger : Singleton<Logger>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Singleton"/> class.
		/// The constructor is private so the design pattern can be enforced,
		/// making it impossible for the user to instantiate this class directly.
		/// </summary>
		private Logger() {; }

		public enum Severity
		{
			Critical, Error, Warning, Notice, Informational, Debug
		}

		/// <summary>
		/// Defines the minium severity each message must have to be shown on
		/// the games remote console.
		/// </summary>
		private static Severity consoleSeverityThreshold
			= Severity.Notice;

		/// <summary>
		/// Defines the minium severity each message must have to be recorded on
		/// the global log file.
		/// TODO: Work in progress
		/// </summary>
		//private static Severity logfileSeverityThreshold
		//	= Severity.Debug;

		/// <summary>
		/// Handles the output to the console and any other active logging
		/// methods.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		internal static void Write(Severity severity, object message, Exception ex = null)
		{
			// Guards the console output to the min threshold defined
			if (severity > consoleSeverityThreshold) return;
			string prefix = $"[{severity.ToString().ToUpper()}] ";

			switch (severity)
			{
				case Severity.Critical:
				case Severity.Error:
					Debug.LogError($"{prefix}{message}");
					break;

				case Severity.Warning:
					Debug.LogWarning($"{prefix}{message}");
					break;

				case Severity.Notice:
				case Severity.Informational:
				case Severity.Debug:
					Debug.Log($"{prefix}{message}");
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");

			}

			if (ex == null) return;

			StringBuilder sb = new StringBuilder();

			sb.AppendLine($" --- EXCEPTION ---------");
			sb.AppendLine($"  {ex.Message}");

			sb.AppendLine($" --- STACKTRACE --------");
			sb.AppendLine($"{ex.StackTrace}");

			sb.AppendLine($" -----------------------");
			Debug.Log(sb.ToString());
		}

#if DEBUG
		/// <summary>
		/// Makes the string smaller if larger than MaxSize.
		/// </summary>
		/// <param name="Input"></param>
		/// <param name="MaxSize"></param>
		internal string GetFileNameEx(string Input)
		{
			// For some reason Path.GetFileName() is not working with
			// [CallerFilePath]. Trying to be OS agnostic..
			return Input.Split((Input.Contains('/') ? '/' : '\\')).Last();
		}
#endif

		/// <summary>
		/// Outputs to the game's console a message with severity level 'NOTICE'.
		/// </summary>
		/// <param name="message"></param>
		public void Log(object message)
			=> Carbon.Logger.Write(Logger.Severity.Notice, message);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'NOTICE'.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Format(string format, params object[] args)
			=> Log(string.Format(format, args));

		/// <summary>
		/// Outputs to the game's console a message with severity level 'WARNING'.
		/// DEBUG IS ENABLED FOR THIS METHOD.
		/// </summary>
		/// <param name="message"></param>
#if DEBUG
		public void Warn(object message,
			[CallerLineNumber] int line = 0,
			[CallerFilePath] string path = null,
			[CallerMemberName] string method = null)
		{
			message = $"{message}\n(file: {GetFileNameEx(path)}, method: {method}, line: {line}]\n";
			Carbon.Logger.Write(Logger.Severity.Warning, message);
		}
#else
		public void Warn(object message)
			=> Carbon.Logger.Write(Logger.Severity.Warning, message);
#endif

		/// <summary>
		/// Outputs to the game's console a message with severity level 'WARNING'.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void WarnFormat(string format, params object[] args)
			=> Warn(string.Format(format, args));

		/// <summary>
		/// Outputs to the game's console a message with severity level 'ERROR'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
#if DEBUG
		public void Error(object message, Exception ex = null,
			[CallerLineNumber] int line = 0,
			[CallerFilePath] string path = null,
			[CallerMemberName] string method = null)
		{
			message = $"{message}\n(file: {GetFileNameEx(path)}, method: {method}, line: {line}]\n";
			Carbon.Logger.Write(Logger.Severity.Error, message, ex);
		}
#else
		public void Error(object message, Exception ex = null)
			=> Carbon.Logger.Write(Logger.Severity.Error, message, ex);
#endif

		/// <summary>
		/// Outputs to the game's console a message with severity level 'ERROR'.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="ex"></param>
		/// <param name="args"></param>
		public void ErrorFormat(string format, Exception ex = null, params object[] args)
			=> Error(string.Format(format, args), ex);
	}
}
