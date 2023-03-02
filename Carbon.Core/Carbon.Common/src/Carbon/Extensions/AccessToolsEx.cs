using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Extensions
{
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
	}
}
