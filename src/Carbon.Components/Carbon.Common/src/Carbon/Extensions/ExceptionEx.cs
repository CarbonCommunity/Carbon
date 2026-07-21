using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;

namespace Carbon.Extensions;

public static class ExceptionEx
{
	private static readonly Regex _missingMethodRegex = new(@"Method not found:\s*(?:'([^']+)'|(.+))", RegexOptions.Compiled);
	private static readonly Regex _missingFieldRegex = new(@"Field not found:\s*(?:'([^']+)'|(.+))", RegexOptions.Compiled);
	private static readonly Regex _typeLoadAssemblyRegex = new(@"from assembly\s+['""]([^'""]+)['""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
	private static readonly ConcurrentDictionary<string, bool> _compatibilityTypeCache = new();

	static ExceptionEx()
	{
		AppDomain.CurrentDomain.AssemblyLoad += (_, _) => _compatibilityTypeCache.Clear();
	}

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

	private static bool TryExtractMissingMember(MissingMemberException exception, out string member)
	{
		var regex = exception switch
		{
			MissingMethodException => _missingMethodRegex,
			MissingFieldException => _missingFieldRegex,
			_ => null
		};
		var match = regex?.Match(exception.Message);
		if (match?.Success != true)
		{
			member = string.Empty;
			return false;
		}

		member = (match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value).Trim();
		return !string.IsNullOrEmpty(member);
	}

	private static bool IsCompatibilityMissingMember(MissingMemberException exception)
	{
		if (!TryExtractMissingMember(exception, out var member))
			return false;

		var parameterIndex = member.IndexOf('(');
		if (parameterIndex >= 0)
			member = member[..parameterIndex];

		var constructorIndex = member.LastIndexOf("..", StringComparison.Ordinal);
		var memberIndex = constructorIndex >= 0 ? constructorIndex : member.LastIndexOf('.');
		if (memberIndex <= 0)
			return false;

		var declaringTypeName = member[..memberIndex];
		var returnTypeIndex = declaringTypeName.IndexOf(' ');
		if (returnTypeIndex >= 0)
			declaringTypeName = declaringTypeName[(returnTypeIndex + 1)..];

		return _compatibilityTypeCache.GetOrAdd(declaringTypeName, static typeName =>
		{
			var declaringType = AccessToolsEx.TypeByName(typeName);
			if (declaringType != null)
				return IsCompatibilityAssembly(declaringType.Assembly.GetName().Name);

			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => IsCompatibilityAssembly(assembly.GetName().Name))
				.SelectMany(AccessToolsEx.GetTypesFromAssembly)
				.Any(type => type.FullName != null && type.FullName.Replace('+', '.').Equals(typeName, StringComparison.Ordinal));
		});
	}

	private static bool IsCompatibilityTypeLoad(TypeLoadException exception)
	{
		var assemblyMatch = _typeLoadAssemblyRegex.Match(exception.Message);
		if (assemblyMatch.Success && IsCompatibilityAssembly(GetSimpleAssemblyName(assemblyMatch.Groups[1].Value)))
			return true;

		if (string.IsNullOrEmpty(exception.TypeName))
			return false;

		var genericDepth = 0;
		var commaIndex = -1;
		for (var i = 0; i < exception.TypeName.Length; i++)
		{
			switch (exception.TypeName[i])
			{
				case '[':
					genericDepth++;
					break;
				case ']':
					genericDepth--;
					break;
				case ',' when genericDepth == 0:
					commaIndex = i;
					break;
			}

			if (commaIndex >= 0)
				break;
		}

		return commaIndex >= 0 && IsCompatibilityAssembly(GetSimpleAssemblyName(exception.TypeName[(commaIndex + 1)..]));
	}

	public static bool TryGetCompatibilityException(this Exception exception, out Exception result)
	{
		var ex = exception;
		while (ex != null)
		{
			if (ex is MissingMemberException missingMember && IsCompatibilityMissingMember(missingMember) ||
				ex is TypeLoadException typeLoad && IsCompatibilityTypeLoad(typeLoad))
			{
				result = ex;
				return true;
			}

			if (ex is BadImageFormatException badImage && !string.IsNullOrEmpty(badImage.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(badImage.FileName)))
			{
				result = ex;
				return true;
			}

			if (ex is FileNotFoundException fileNotFound && !string.IsNullOrEmpty(fileNotFound.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileNotFound.FileName)))
			{
				result = ex;
				return true;
			}

			if (ex is FileLoadException fileLoad && !string.IsNullOrEmpty(fileLoad.FileName) &&
				IsCompatibilityAssembly(GetSimpleAssemblyName(fileLoad.FileName)))
			{
				result = ex;
				return true;
			}

			ex = ex.InnerException;
		}

		result = null;
		return false;
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
		return TryGetCompatibilityException(exception, out _);
	}

	public static Exception GetCompatibilityException(this Exception exception)
	{
		return TryGetCompatibilityException(exception, out var result) ? result : exception;
	}

	public static string GetCompatibilityMessage(this Exception exception)
	{
		return exception.GetCompatibilityMessage(exception.GetCompatibilityException());
	}

	public static string GetCompatibilityMessage(this Exception exception, Exception compatibilityException)
	{
		var member = exception.ExtractMissingMember(compatibilityException);
		var suffix = string.IsNullOrEmpty(member) ? string.Empty : $" ('{member}')";
		return $"This usually means the Rust/Carbon/plugin versions are out of sync{suffix}. Try updating Carbon, the module/plugin, or the Rust server.";
	}

	public static string ExtractMissingMember(this Exception exception)
	{
		if (!TryGetCompatibilityException(exception, out var compatibilityException))
			return string.Empty;

		return exception.ExtractMissingMember(compatibilityException);
	}

	public static string ExtractMissingMember(this Exception exception, Exception compatibilityException)
	{
		if (compatibilityException is MissingMemberException missingMember)
			return TryExtractMissingMember(missingMember, out var member) ? member : missingMember.Message;

		if (compatibilityException is TypeLoadException typeLoad)
			return string.IsNullOrEmpty(typeLoad.TypeName) ? compatibilityException.Message : typeLoad.TypeName;

		if (compatibilityException is BadImageFormatException badImage)
			return string.IsNullOrEmpty(badImage.FileName) ? compatibilityException.Message : badImage.FileName;

		if (compatibilityException is FileNotFoundException fileNotFound)
			return GetSimpleAssemblyName(fileNotFound.FileName);

		if (compatibilityException is FileLoadException fileLoad)
			return GetSimpleAssemblyName(fileLoad.FileName);

		return string.Empty;
	}
}
