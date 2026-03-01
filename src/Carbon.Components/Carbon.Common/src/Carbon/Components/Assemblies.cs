using System.Collections.Concurrent;

namespace Carbon;

public class Assemblies
{
	public static RuntimeAssemblyBank Plugins { get; } = new();
	public static RuntimeAssemblyBank Modules { get; } = new();
	public static RuntimeAssemblyBank Extensions { get; } = new();
	public static RuntimeAssemblyBank Harmony { get; } = new();

	public class RuntimeAssembly
	{
		public Assembly CurrentAssembly { get; internal set; }
		public bool IsProfiledAssembly { get; internal set; }
		public string Location { get; internal set; }
		public List<RuntimeAssembly> History { get; internal set; }
	}

	public class RuntimeAssemblyBank : ConcurrentDictionary<string, RuntimeAssembly>
	{
		public RuntimeAssembly Get(string key)
		{
			TryGetValue(key, out var existent);
			return existent;
		}
		public KeyValuePair<string, RuntimeAssembly> Find(Assembly assembly)
		{
			foreach(var a in this)
			{
				if(a.Value.CurrentAssembly == assembly)
				{
					return a;
				}

				foreach(var aHistory in a.Value.History)
				{
					if(aHistory.CurrentAssembly == assembly)
					{
						return new(a.Key, aHistory);
					}
				}
			}

			return default;
		}
		public void Update(string key, Assembly assembly, string location, bool isProfiledAssembly = false)
		{
			if (string.IsNullOrEmpty(key))
			{
				Logger.Warn($"RuntimeAssemblyBank.Update key == null");
				return;
			}

			if (assembly == null)
			{
				Logger.Warn($"RuntimeAssemblyBank.Update assembly == null");
				return;
			}

			AddOrUpdate(key, _ => new RuntimeAssembly
			{
				CurrentAssembly = assembly,
				IsProfiledAssembly = isProfiledAssembly,
				Location = location,
				History = []
			},
				(_, existent) =>
				{
					if (existent.CurrentAssembly != null)
					{
						existent.History.Insert(0, new RuntimeAssembly
						{
							CurrentAssembly = existent.CurrentAssembly,
							IsProfiledAssembly = existent.IsProfiledAssembly,
							Location = existent.Location
						});
					}

					existent.CurrentAssembly = assembly;
					existent.Location = location;
					existent.IsProfiledAssembly = isProfiledAssembly;
					return existent;
				});
		}
	}
}
