using System.Reflection.Emit;
using Facepunch;
using HarmonyLib;

namespace Carbon.Extensions;

public static class AccessToolsEx
{
	private static Dictionary<string, Type> searchCache = [];

	public static Type TypeByName(string name)
	{
		if (searchCache.TryGetValue(name, out Type type))
		{
			return type;
		}
		return searchCache[name] = Type.GetType(name, throwOnError: false) ?? AccessTools.TypeByName(name) ?? SearchTypeByName(name);
	}

	private static Type SearchTypeByName(string name)
	{
		var type = Type.GetType(name, throwOnError: false);
		if (type != null)
			return type;

		if (name.Contains('`'))
		{
			if (name.Contains("["))
			{
				name = name.Replace("[", "<").Replace("]", ">");
			}

			var bracketIndex = name.IndexOf('<');
			if (bracketIndex < 0 || name[^1] != '>')
			{
				return FindTypeByFullName(name);
			}

			var genericTypeDefName = name[..bracketIndex];

			using var genericArgs = Pool.Get<PooledList<string>>();
			SplitGenericArguments(name.Substring(bracketIndex + 1, name.Length - bracketIndex - 2), genericArgs);

			// Resolve each argument recursively so nested generics (e.g. List`1<Outer/Inner`1<T>>) work
			var argTypes = genericArgs.Select(TypeByName).ToArray();

			if (argTypes.Any(x => x == null))
			{
				Logger.Warn("AccessTools.TypeByName: Failed to resolve one or more generic arguments for " + name);
				return null;
			}

			var genericDef = AccessTools.TypeByName(genericTypeDefName) ?? FindTypeByFullName(genericTypeDefName);
			if (genericDef == null)
			{
				Logger.Warn("AccessTools.TypeByName: Could not find generic type definition " + genericTypeDefName);
				return null;
			}

			if (!genericDef.IsGenericTypeDefinition)
			{
				genericDef = genericDef.GetGenericTypeDefinition();
			}

			try
			{
				return genericDef.MakeGenericType(argTypes);
			}
			catch (Exception ex)
			{
				Logger.Error("AccessTools.TypeByName: Failed to MakeGenericType for " + name + " - " + ex.Message);
			}
		}

		var types = AccessTools.AllTypes();
		type = types.FirstOrDefault(t => t.FullName == name);
		if (type != null)
			return type;

		return types.FirstOrDefault(t => t.Name == name) ?? FindTypeByFullName(name);
	}

	private static Type FindTypeByFullName(string name)
	{
		// Cecil-style nested type names use '/', reflection uses '+'
		if (!name.Contains('/'))
		{
			return null;
		}

		var reflectionName = name.Replace('/', '+');
		return AccessTools.TypeByName(reflectionName) ?? AccessTools.AllTypes().FirstOrDefault(t => t.FullName == reflectionName);
	}

	private static void SplitGenericArguments(string value, List<string> output)
	{
		var depth = 0;
		var start = 0;

		for (var i = 0; i < value.Length; i++)
		{
			switch (value[i])
			{
				case '<':
					depth++;
					break;

				case '>':
					depth--;
					break;

				case ',' when depth == 0:
					output.Add(value[start..i].Trim());
					start = i + 1;
					break;
			}
		}

		output.Add(value[start..].Trim());
	}

	public static IEnumerable<Type> AllTypes()
	{
		return AllAssemblies().SelectMany((Assembly a) => GetTypesFromAssembly(a));
	}

	public static Type[] GetTypesFromAssembly(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where((Type type) => (object)type != null).ToArray();
		}
	}

	public static IEnumerable<Assembly> AllAssemblies()
	{
		return from a in AppDomain.CurrentDomain.GetAssemblies()
			   where !a.FullName.StartsWith("Microsoft.VisualStudio")
			   select a;
	}

	public static IEnumerable<Type> GetConstraints(Type type)
	{
		// generics with only one type will be supported
		Type[] generics = type.GetGenericArguments();

		if (generics.Count() > 1)
			throw new Exception($"GetConstraints only supports generics with one type");

		Type generic = generics.First();
		return generic.GetGenericParameterConstraints();
	}

	public static IEnumerable<Type> MatchConstrains(IEnumerable<Type> constrains)
	{
		IEnumerable<Type> interfaces = constrains.Where(x => x.IsInterface);
		Type @base = constrains.Single(x => !x.IsInterface);

		return AccessToolsEx.AllTypes().Where(type =>
			type.IsSubclassOf(@base) && type.GetInterfaces().Intersect(interfaces).Any());
	}

	public static CodeInstruction WithLabel(this CodeInstruction instruction, Label label)
	{
		instruction.labels.Add(label);
		return instruction;
	}
}
