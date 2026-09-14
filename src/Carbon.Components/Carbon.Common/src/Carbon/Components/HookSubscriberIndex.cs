using Carbon.Base.Interfaces;

namespace Carbon;

public static class HookSubscriberIndex
{
	private const BindingFlags DefaultFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

	private static readonly Dictionary<uint, BaseHookable[]> _index = new();
	private static int _version;
	private static int _builtVersion = -1;

	public static int Version => _version;
	public static int BuiltVersion => _builtVersion;
	public static IReadOnlyDictionary<uint, BaseHookable[]> Current => _index;

	public static void Invalidate()
	{
		Interlocked.Increment(ref _version);
	}

	public static BaseHookable[] Get(uint hookId)
	{
		return Get(hookId, DefaultFlags);
	}

	public static BaseHookable[] Get(uint hookId, BindingFlags flag)
	{
		if (_builtVersion != _version)
		{
			Refresh(flag);
		}

		if (!_index.TryGetValue(hookId, out var subscribers))
		{
			_index[hookId] = subscribers = Collect(hookId);
		}

		return subscribers;
	}

	private static void Refresh(BindingFlags flag)
	{
		_index.Clear();

		var modules = Community.Runtime.ModuleProcessor.Modules;

		for (int i = 0; i < modules.Count; i++)
		{
			var hookable = modules[i];

			if (hookable is IModule module && !module.IsEnabled())
			{
				continue;
			}

			EnsureCache(hookable, flag);
		}

		var packages = ModLoader.Packages;

		for (int i = 0; i < packages.Count; i++)
		{
			var plugins = packages[i].Plugins;

			if (plugins == null)
			{
				continue;
			}

			for (int o = 0; o < plugins.Count; o++)
			{
				EnsureCache(plugins[o], flag);
			}
		}

		_builtVersion = _version;
	}

	private static void EnsureCache(BaseHookable hookable, BindingFlags flag)
	{
		if (hookable.HasBuiltHookCache || hookable.HookableType == null || hookable.Hooks == null)
		{
			return;
		}

		try
		{
			hookable.BuildHookCache(flag);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed building hook cache for '{hookable.Name} v{hookable.Version}'", ex);
		}
	}

	private static BaseHookable[] Collect(uint hookId)
	{
		var list = Facepunch.Pool.Get<List<BaseHookable>>();
		var modules = Community.Runtime.ModuleProcessor.Modules;

		for (int i = 0; i < modules.Count; i++)
		{
			var hookable = modules[i];

			if (hookable is IModule module && !module.IsEnabled())
			{
				continue;
			}

			if (hookable.HookPool != null && hookable.HookPool.ContainsKey(hookId))
			{
				list.Add(hookable);
			}
		}

		var packages = ModLoader.Packages;

		for (int i = 0; i < packages.Count; i++)
		{
			var plugins = packages[i].Plugins;

			if (plugins == null)
			{
				continue;
			}

			for (int o = 0; o < plugins.Count; o++)
			{
				var plugin = plugins[o];

				if (plugin.HookPool != null && plugin.HookPool.ContainsKey(hookId))
				{
					list.Add(plugin);
				}
			}
		}

		var subscribers = list.Count == 0 ? Array.Empty<BaseHookable>() : list.ToArray();
		Facepunch.Pool.FreeUnmanaged(ref list);
		return subscribers;
	}
}
