using System.IO;
using System.Text.RegularExpressions;

namespace Carbon.Extensions;

public static class ExceptionEx
{
	private static readonly Regex _missingMethodRegex = new(@"Method not found:\s*(?:'([^']+)'|(.+))", RegexOptions.Compiled);
	private static readonly Regex _missingFieldRegex = new(@"Field not found:\s*(?:'([^']+)'|(.+))", RegexOptions.Compiled);

	private static string GetSimpleAssemblyName(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
			return string.Empty;

		var name = Path.GetFileNameWithoutExtension(fileName);
		var commaIndex = name.IndexOf(',');
		if (commaIndex >= 0)
			name = name[..commaIndex];

		return name.Trim();
	}

	private static bool IsCompatibilityAssembly(string assemblyName)
	{
		if (string.IsNullOrEmpty(assemblyName))
			return false;

		return assemblyName.Equals("Assembly-CSharp", StringComparison.OrdinalIgnoreCase) ||
			assemblyName.Equals("Rust", StringComparison.OrdinalIgnoreCase) ||
			assemblyName.StartsWith("Rust.", StringComparison.OrdinalIgnoreCase) ||
			assemblyName.Equals("Carbon", StringComparison.OrdinalIgnoreCase) ||
			assemblyName.StartsWith("Carbon.", StringComparison.OrdinalIgnoreCase);
	}

	public static string GetFullStackTrace(this Exception exception, bool mainMessage = true)
	{
		var fullStackTrace = mainMessage ? exception.ToString() : exception.StackTrace;
		var innerException = exception.InnerException;

		while (innerException != null)
		{
			fullStackTrace += "\n  Inner exception:\n  " + innerException;
			innerException = innerException.InnerException;
		}

		return fullStackTrace;
	}

	public static bool IsCompatibilityError(this Exception exception)
	{
		var ex = exception;
		while (ex != null)
		{
			if (ex is MissingMemberException or TypeLoadException or BadImageFormatException)
				return true;

			if (ex is FileNotFoundException fileNotFound && !string.IsNullOrEmpty(fileNotFound.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileNotFound.FileName)))
					return true;

			if (ex is FileLoadException fileLoad && !string.IsNullOrEmpty(fileLoad.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileLoad.FileName)))
					return true;

			ex = ex.InnerException;
		}

		return false;
	}

	public static Exception GetCompatibilityException(this Exception exception)
	{
		var ex = exception;
		while (ex != null)
		{
			if (ex is MissingMemberException or TypeLoadException or BadImageFormatException)
				return ex;

			if (ex is FileNotFoundException fileNotFound && !string.IsNullOrEmpty(fileNotFound.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileNotFound.FileName)))
					return ex;

			if (ex is FileLoadException fileLoad && !string.IsNullOrEmpty(fileLoad.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileLoad.FileName)))
					return ex;

			ex = ex.InnerException;
		}

		return exception;
	}

	public static string GetCompatibilityMessage(this Exception exception)
	{
		var member = exception.GetCompatibilityException().ExtractMissingMember();
		var suffix = string.IsNullOrEmpty(member) ? string.Empty : $" ('{member}')";
		return $"This usually means the Rust/Carbon/plugin versions are out of sync{suffix}. Try updating Carbon, the module/plugin, or the Rust server.";
	}

	public static string ExtractMissingMember(this Exception exception)
	{
		var ex = exception;
		while (ex != null)
		{
			if (ex is MissingFieldException missingField)
			{
				var match = _missingFieldRegex.Match(missingField.Message);
				if (match.Success)
					return match.Groups[1].Success ? match.Groups[1].Value.Trim() : match.Groups[2].Value.Trim();

				return missingField.Message;
			}

			if (ex is MissingMethodException missingMethod)
			{
				var match = _missingMethodRegex.Match(missingMethod.Message);
				if (match.Success)
					return match.Groups[1].Success ? match.Groups[1].Value.Trim() : match.Groups[2].Value.Trim();

				return missingMethod.Message;
			}

			if (ex is MissingMemberException missingMember)
			{
				return missingMember.Message;
			}

			if (ex is TypeLoadException typeLoad)
			{
				return string.IsNullOrEmpty(typeLoad.TypeName) ? ex.Message : typeLoad.TypeName;
			}

			if (ex is BadImageFormatException badImage)
			{
				return string.IsNullOrEmpty(badImage.FileName) ? ex.Message : badImage.FileName;
			}

			if (ex is FileNotFoundException fileNotFound && !string.IsNullOrEmpty(fileNotFound.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileNotFound.FileName)))
			{
				return GetSimpleAssemblyName(fileNotFound.FileName);
			}

			if (ex is FileLoadException fileLoad && !string.IsNullOrEmpty(fileLoad.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileLoad.FileName)))
			{
				return GetSimpleAssemblyName(fileLoad.FileName);
			}

			ex = ex.InnerException;
		}

		return string.Empty;
	}
}
