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
		return searchCache[name] = AccessTools.TypeByName(name) ?? SearchTypeByName(name);
	}

	private static Type SearchTypeByName(string name)
	{
		var type = Type.GetType(name, throwOnError: false);
		if (type != null)
			return type;

		if (name.Contains('`'))
		{
			var bracketIndex = name.IndexOf('<');
			var genericTypeDefName = name[..bracketIndex];
			var genericArgPart = name[bracketIndex..];

			using var genericArgs = Pool.Get<PooledList<string>>();
			genericArgs.AddRange(genericArgPart.Replace("<", string.Empty).Replace(">", string.Empty).Split(','));

			var argTypes = genericArgs
				.Select(arg => AccessTools.TypeByName(arg))
				.ToArray();

			if (argTypes.Length != genericArgs.Count)
			{
				Logger.Warn("AccessTools.TypeByName: Failed to resolve one or more generic arguments for " + name);
				return null;
			}

			var genericDef = AccessTools.TypeByName(genericTypeDefName);
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

		return types.FirstOrDefault(t => t.Name == name);
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
			Logger.Debug($"AccessTools.GetTypesFromAssembly: assembly {assembly} => {ex}");
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
