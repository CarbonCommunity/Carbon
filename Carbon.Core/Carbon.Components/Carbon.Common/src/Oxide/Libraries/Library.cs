namespace Oxide.Core.Libraries;

public class Library : IDisposable
{
	internal IDictionary<string, MethodInfo> functions;
	internal IDictionary<string, PropertyInfo> properties;

	public virtual bool IsGlobal { get; }

	public Exception LastException { get; protected set; }

	public Library()
	{
		var libraryId = GetType().Name;

		if (!OxideMod._libraryCache.ContainsKey(libraryId))
		{
			OxideMod._libraryCache.TryAdd(libraryId, this);
		}

		functions = new Dictionary<string, MethodInfo>();
		properties = new Dictionary<string, PropertyInfo>();
		var type = this.GetType();

		foreach (var method in type.GetMethods())
		{
			LibraryFunction libraryFunction = null;

			try
			{
				libraryFunction = method.GetCustomAttribute<LibraryFunction>(true);

				if (libraryFunction == null)
					continue;
			}
			catch (TypeLoadException)
			{
				continue;
			}

			var key = libraryFunction?.Name ?? method.Name;

			if (functions.ContainsKey(key))
				Interface.Oxide.LogError(type.FullName + " library tried to register an already registered function: " + key);
			else
				functions[key] = method;
		}
		foreach (var property in type.GetProperties())
		{
			LibraryProperty libraryProperty = null;

			try
			{
				libraryProperty = property.GetCustomAttribute<LibraryProperty>(true);

				if (libraryProperty == null)
					continue;
			}
			catch (TypeLoadException)
			{
				continue;
			}

			var key = libraryProperty?.Name ?? property.Name;

			if (properties.ContainsKey(key))
				Interface.Oxide.LogError("{0} library tried to register an already registered property: {1}", (object) type.FullName, (object) key);
			else
				properties[key] = property;
		}
	}

	public static implicit operator bool(Library library)
	{
		return library != null;
	}
	public static bool operator !(Library library)
	{
		return !library;
	}

	public virtual void Dispose() { }
	public virtual void Shutdown() { }

	public MethodInfo GetFunction(string name)
	{
		return functions.TryGetValue(name, out var methodInfo) ? methodInfo : null;
	}
	public PropertyInfo GetProperty(string name)
	{
		return properties.TryGetValue(name, out var propertyInfo) ? propertyInfo : null;
	}

	public IEnumerable<string> GetFunctionNames() => functions.Keys;
	public IEnumerable<string> GetPropertyNames() => properties.Keys;
}
