using System.Reflection;

namespace Carbon.Utility;

internal abstract class Program
{
	private static readonly HashSet<Assembly> CachedAssemblies = [];
	private static readonly HashSet<string> Blacklist = [];
	private static readonly Lock ResolverLock = new();

	internal static Assembly? AssemblyResolver(object? sender, ResolveEventArgs args)
	{
		return ResolveAssembly(args.Name);
	}

	internal static Assembly? ResolveAssembly(string name)
	{
		lock (ResolverLock)
		{
			return ResolveAssemblyCore(name);
		}
	}

	private static Assembly? ResolveAssemblyCore(string name)
	{
		if (Blacklist.Contains(name))
		{
			return null;
		}

		Assembly? assembly = null;
		var assemblyName = new AssemblyName(name);

		var fileName = $"{assemblyName.Name}.dll";

		var cached = CachedAssemblies.FirstOrDefault(item => item.GetName().Name == assemblyName.Name);
		if (cached != null)
		{
			return cached;
		}

		var location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(fileName));
		if (File.Exists(location))
		{
			assembly = Assembly.LoadFrom(location);
		}

		location = Path.Combine(Generation.Program.Arguments.ManagedFolder, Path.GetFileName(fileName));
		if (File.Exists(location))
		{
			assembly = Assembly.LoadFrom(location);
		}

		if (assembly != null)
		{
			CachedAssemblies.Add(assembly);
		}
		else
		{
			Blacklist.Add(name);
		}

		return assembly;
	}
}
