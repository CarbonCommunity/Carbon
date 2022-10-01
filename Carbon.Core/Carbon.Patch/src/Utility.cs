using System;
using System.Reflection;

namespace Carbon.Patch
{
	internal class Utility
	{
		internal static void LogNone (object message)
			=> Console.WriteLine($"{message}");

		internal static void LogInformation (object message)
			=> ConsoleWrite($"[INFO] {message}");

		internal static void LogWarning (object message)
			=> ConsoleWrite($"[WARN] {message}");

		internal static void LogError (object message)
			=> ConsoleWrite($"[ERROR] {message}");

		private static void ConsoleWrite (object message)
			=> Console.WriteLine($"[{GetProduct()}] {message}");

		internal static string GetAssemblyName ()
			=> typeof(Program).Assembly.GetName().Name;

		internal static string GetProduct ()
		{
			try
			{
				Assembly asm = typeof(Program).Assembly;
				object [] attr = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
				return ((AssemblyProductAttribute)attr [ 0 ]).Product;
			}
			catch { return string.Empty; }
		}

		internal static string GetCopyright ()
		{
			try
			{
				Assembly asm = typeof(Program).Assembly;
				object [] attr = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
				return ((AssemblyCopyrightAttribute)attr [ 0 ]).Copyright;
			}
			catch { return string.Empty; }
		}

		internal static string GetVersion ()
		{
			try
			{
				Assembly asm = typeof(Program).Assembly;
				object [] attr = asm.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true);
				return ((AssemblyInformationalVersionAttribute)attr [ 0 ]).InformationalVersion;
			}
			catch { return "Unknown"; }
		}

		internal static string GetSmallerString (string Input, int MaxSize = 60)
		{
			if (Input.Length <= MaxSize) return Input;
			return $"(...){Input.Substring(Input.Length - MaxSize + 5)}";
		}
	}
}
