using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace Carbon.Utility;

internal static class Tools
{
	private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

	internal static MethodInfo? MethodByNameEx(Type? typeName, string? methodName, string[]? methodArguments = null)
	{
		MethodInfo? retval;

		try // #1 basic fast reflection search
		{
			if (typeName == null || methodName == null)
			{
				return null;
			}

			var count = AccessTools.GetDeclaredMethods(typeName).Count(method => method.Name == methodName);
			if (count > 1)
			{
				throw new Exception();
			}

			retval = AccessTools.Method(typeName, methodName);
			if (retval != null)
			{
				return retval;
			}
		}
		catch (Exception)
		{
			/* do nothing */
		}

		try // #1 basic slow reflection search [less accurate]
		{
			if (typeName == null || methodName == null)
			{
				return null;
			}

			retval = AccessTools.Method(typeName, methodName);
			if (retval != null && retval.GetParameters().Length == (methodArguments?.Length ?? 0))
			{
				return retval;
			}
		}
		catch (Exception)
		{
			/* do nothing */
		}

		if (methodArguments == null)
		{
			return null;
		}

		try // #2 slower reflection search using argument array
		{
			var types = new List<Type>();

			for (var argumentIndex = 0; argumentIndex < methodArguments.Length; argumentIndex++)
			{
				var argument = methodArguments[argumentIndex];
				var type = TypeByNameEx(argument);
				if (type == null)
				{
					throw new Exception();
				}

				types.Add(type);
			}

			retval = AccessTools.Method(typeName, methodName, types.ToArray());
			if (retval != null && retval.GetParameters().Length == methodArguments.Length)
			{
				return retval;
			}
		}
		catch (Exception)
		{
			/* do nothing */
		}

		try // #3 slowest iteration search [most accurate]
		{
			IEnumerable<MethodInfo> methods = AccessTools.GetDeclaredMethods(typeName)
				.Where(method => method.Name == methodName).Where(method => method.GetParameters().Length == methodArguments.Length);

			foreach (var method in methods)
			{
				// skip if the method paramaters don't match with signature
				//ParameterInfo[] parameters = method.GetParameters();
				//if (parameters.Length != methodArguments.Length) continue;

				var x = 0;
				retval = method;

				var methodParameters = method.GetParameters();
				for (var parameterIndex = 0; parameterIndex < methodParameters.Length; parameterIndex++)
				{
					var parameter = methodParameters[parameterIndex];
					var type = TypeByNameEx(methodArguments[x++]);
					if (type == null || parameter.ParameterType == type)
					{
						continue;
					}

					retval = null;
					break;
				}

				if (retval != null)
				{
					return retval;
				}
			}
		}
		catch (Exception)
		{
			/* do nothing */
		}

		return null;
	}

	internal static Type? TypeByNameEx(string typeName)
	{
		if (TypeCache.TryGetValue(typeName, out var retval))
		{
			return retval;
		}

		retval = TypeByNameEx1(typeName);
		if (retval != null)
		{
			return retval;
		}

		retval = TypeByNameEx2(typeName);
		if (retval != null)
		{
			return retval;
		}

		// Match match = Regex.Match(typeName, @"^((?:[\.\w]+))(`\d)?(?:<([\.,\w]+)>)?");
		return null;
	}

	internal static Type? TypeByNameEx1(string typeName)
	{
		try
		{
			var retval = AccessTools.TypeByName(typeName);

			if (retval != null)
			{
				TypeCache.TryAdd(typeName, retval);
				return retval;
			}
		}
		catch (Exception)
		{
			/* basic search */
		}

		return null;
	}

	internal static Type? TypeByNameEx2(string typeName)
	{
		var split = typeName.Split(['/', '+'], StringSplitOptions.RemoveEmptyEntries);
		if (split.Length <= 1)
		{
			return null;
		}

		try
		{
			var retval = AccessTools.TypeByName(split[0]);
			for (var x = 1; x < split.Length; x++)
			{
				if (retval == null)
				{
					return null;
				}

				retval = retval.GetNestedType(split[x], BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			}

			if (retval != null)
			{
				TypeCache.TryAdd(typeName, retval);
				return retval;
			}
		}
		catch (Exception)
		{
			/* nested types didn't work */
		}

		return null;
	}

	internal static string TypeNameSanitizerEx(string type)
	{
		var translator = new Dictionary<string, string>
		{
			{ @"[\/|\+]", "." },
			{ @"[\&]", string.Empty },
            //{ @"(`\d)", "<>" },
            { @",\s*[^\[\]]+?,\s*Version=[^\[\]]*?,\s*Culture=[^\[\]]*?,\s*PublicKeyToken=[^\[\],]+", "" },
			{ @"\]\s*,\s*\[", "," },
			{ @"\[\[", "[" },
			{ @"\]\]", "]" },
			{ @"`\d+[<\[](.+)[>\]]", @"<$1>" },
			{ @"Oxide.Core.Interface", "Interface" },
		};

		var retval = type;
		foreach (var sentence in translator)
		{
			retval = Regex.Replace(retval, sentence.Key, sentence.Value);
		}

		return retval;
	}

	internal static string JoinStringEx(string[] args, string format = "{0}", string separator = ", ")
	{
		var retvar = new List<string>();
		for (var i = 0; i < args.Length; i++)
		{
			var argument = args[i];
			retvar.Add(string.Format(format, TypeNameSanitizerEx(argument)));
		}

		return string.Join(separator, retvar);
	}

	internal static int ArgumentParser(string input, out string[] argvar, out string? retvar)
	{
		argvar = [];
		retvar = null;

		if (string.IsNullOrEmpty(input))
		{
			return 0;
		}

		// strip whitespace
		var brown = StripWhitespace(input);
		var split = brown.Split(["=>"], StringSplitOptions.RemoveEmptyEntries);

		argvar = split[0].Split(',');
		if (split.Length > 1)
		{
			retvar = split[1];
		}

		return argvar.Length;
	}

	public static string? GetMethodMsilHash(MethodInfo method)
	{
		try
		{
			return Sha1(method.GetMethodBody()?.GetILAsByteArray());
		}
		catch (Exception ex)
		{
			Logger.Error($"GetMethodMSILHash failed for {method?.Name} [{method?.DeclaringType?.Name}] ({ex.Message})\n{ex.StackTrace}");
			return null;
		}
	}

	public static string? Md5(byte[]? raw)
	{
		if (raw == null || raw.Length == 0)
		{
			return null;
		}

		using var md5 = MD5.Create();
		var hash = md5.ComputeHash(raw);
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}

	public static string? Sha1(byte[]? raw)
	{
		if (raw == null || raw.Length == 0)
		{
			return null;
		}

		using var sha1 = SHA1.Create();
		var bytes = sha1.ComputeHash(raw);
		return string.Concat(bytes.Select(b => b.ToString("x2"))).ToLower();
	}

	private static string StripWhitespace(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}

		var buffer = new char[input.Length];
		var count = 0;
		for (var i = 0; i < input.Length; i++)
		{
			var c = input[i];
			if (!char.IsWhiteSpace(c))
			{
				buffer[count++] = c;
			}
		}

		return new string(buffer, 0, count);
	}
}
