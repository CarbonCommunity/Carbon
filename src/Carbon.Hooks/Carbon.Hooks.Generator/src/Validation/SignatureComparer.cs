using System.Text;
using Carbon.Utility;

namespace Carbon.Validation;

/// <summary>
///     Compares and converts type name spellings between the patcher's Cecil-style strings
///     ("Ns.Outer/Inner", "List`1&lt;T&gt;") and reflection's ("Ns.Outer+Inner", "List`1[T]").
/// </summary>
internal static class SignatureComparer
{
	public static bool TypeNamesMatch(string? left, string? right)
	{
		if (left == null || right == null)
		{
			return left == right;
		}

		return string.Equals(Tools.TypeNameSanitizerEx(left), Tools.TypeNameSanitizerEx(right), StringComparison.Ordinal);
	}

	public static bool ParametersMatch(IReadOnlyList<string> left, IReadOnlyList<string> right)
	{
		if (left.Count != right.Count)
		{
			return false;
		}

		for (var i = 0; i < left.Count; i++)
		{
			if (!TypeNamesMatch(left[i], right[i]))
			{
				return false;
			}
		}

		return true;
	}

	public static bool ParametersMatch(System.Reflection.ParameterInfo[] left, IReadOnlyList<string> right)
	{
		if (left.Length != right.Count)
		{
			return false;
		}

		for (var i = 0; i < left.Length; i++)
		{
			// Compare patcher spellings on both sides; reflection's ToString() diverges on nested
			// generics in ways the sanitizer cannot normalize.
			if (!TypeNamesMatch(PatcherTypeName(left[i].ParameterType), right[i]))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     Builds the patcher-style (Cecil FullName) spelling of a runtime type, the format written
	///     into OPJ signature fields.
	/// </summary>
	public static string PatcherTypeName(Type type)
	{
		if (type.IsByRef)
		{
			return PatcherTypeName(type.GetElementType()!) + "&";
		}

		if (type.IsPointer)
		{
			return PatcherTypeName(type.GetElementType()!) + "*";
		}

		if (type.IsArray)
		{
			var rank = type.GetArrayRank();
			return PatcherTypeName(type.GetElementType()!) + "[" + new string(',', Math.Max(rank - 1, 0)) + "]";
		}

		if (type.IsGenericParameter)
		{
			return type.Name;
		}

		if (type.IsConstructedGenericType)
		{
			var arguments = type.GetGenericArguments();
			return PatcherTypeName(type.GetGenericTypeDefinition()) + "<" + string.Join(",", arguments.Select(PatcherTypeName)) + ">";
		}

		var builder = new StringBuilder();
		AppendPlainName(builder, type);
		return builder.ToString();
	}

	private static void AppendPlainName(StringBuilder builder, Type type)
	{
		if (type.DeclaringType != null)
		{
			AppendPlainName(builder, type.DeclaringType);
			builder.Append('/').Append(type.Name);
			return;
		}

		if (!string.IsNullOrEmpty(type.Namespace))
		{
			builder.Append(type.Namespace).Append('.');
		}

		builder.Append(type.Name);
	}

	/// <summary>
	///     Maps a runtime method's accessibility onto the patcher's MethodExposure values.
	/// </summary>
	public static Projects.Oxide.Oxide.MethodSignature.MethodExposure GetExposure(System.Reflection.MethodBase method)
	{
		if (method.IsPublic)
		{
			return Projects.Oxide.Oxide.MethodSignature.MethodExposure.Public;
		}

		if (method.IsPrivate)
		{
			return Projects.Oxide.Oxide.MethodSignature.MethodExposure.Private;
		}

		if (method.IsFamily)
		{
			return Projects.Oxide.Oxide.MethodSignature.MethodExposure.Protected;
		}

		return Projects.Oxide.Oxide.MethodSignature.MethodExposure.Internal;
	}

	/// <summary>
	///     Damerau-free Levenshtein distance, used to rank likely renames.
	/// </summary>
	public static int NameDistance(string left, string right)
	{
		var previous = new int[right.Length + 1];
		var current = new int[right.Length + 1];
		for (var j = 0; j <= right.Length; j++)
		{
			previous[j] = j;
		}

		for (var i = 1; i <= left.Length; i++)
		{
			current[0] = i;
			for (var j = 1; j <= right.Length; j++)
			{
				var substitution = previous[j - 1] + (left[i - 1] == right[j - 1] ? 0 : 1);
				current[j] = Math.Min(Math.Min(previous[j] + 1, current[j - 1] + 1), substitution);
			}

			(previous, current) = (current, previous);
		}

		return previous[right.Length];
	}
}
