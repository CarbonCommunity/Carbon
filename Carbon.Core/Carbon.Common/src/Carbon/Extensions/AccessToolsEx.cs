/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Extensions;

public static class AccessToolsEx
{
	public static Type TypeByName(string name)
	{
		Type type = Type.GetType(name, throwOnError: false);
		if ((object)type == null)
		{
			type = AllTypes().FirstOrDefault((Type t) => t.FullName == name);
		}

		if ((object)type == null)
		{
			type = AllTypes().FirstOrDefault((Type t) => t.Name == name);
		}

		if ((object)type == null)
		{
			Logger.Debug("AccessTools.TypeByName: Could not find type named " + name);
		}

		return type;
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

	public static List<Type> GetConstraints(Type type)
	{
		// generics with only one type will be supported
		Type[] generics = type.GetGenericArguments();

		if (generics.Count() > 1)
			throw new Exception($"GetConstraints only supports generics with one type");

		Type generic = generics.First();
		return generic.GetGenericParameterConstraints().ToList();
	}

	public static List<Type> MatchConstrains(List<Type> constrains)
	{
		IEnumerable<Type> interfaces = constrains.Where(x => x.IsInterface);
		Type @base = constrains.Single(x => !x.IsInterface);

		return AccessToolsEx.AllTypes().Where(type =>
			type.IsSubclassOf(@base) && type.GetInterfaces().Intersect(interfaces).Any()).ToList();
	}
}
