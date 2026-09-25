using Carbon.Base.Interfaces;
using Facepunch;

namespace Carbon.Components;

/// <summary>
/// Owns every loaded module and drives their lifecycle (init, load, server init, save, shutdown).
/// Modules come from Carbon.Common itself and from module assemblies in carbon/managed/modules.
/// </summary>
public class ModuleRegistry : IDisposable
{
	/// <summary>All registered modules, enabled or not.</summary>
	public List<BaseHookable> All { get; } = new(200);

	public int Count => All.Count;
	public int EnabledCount => All.Count(x => x is IModule module && module.IsEnabled());

	public void Init()
	{
		Community.Runtime.Events.Subscribe(CarbonEvent.ModuleLoaded, e =>
		{
			if (e is not ModuleEventArgs m)
			{
				return;
			}

			Build(m.Payload.ToString(), (m.Data as IReadOnlyList<Type>).ToArray());
		});
		Community.Runtime.Events.Subscribe(CarbonEvent.ModuleUnloaded, e =>
		{
			if (e is not ModuleEventArgs { Payload: string context })
			{
				return;
			}

			foreach (var module in All.OfType<BaseModule>().ToArray())
			{
				if (module.Context is string moduleContext && moduleContext.Equals(context))
				{
					module.Shutdown();
				}
			}
		});

		Build(typeof(Community).Assembly.GetExportedTypes());

		foreach (var type in Community.Runtime.AssemblyEx.Modules.Shared)
		{
			Build(type.Value, type.Key);
		}
	}

	public T Get<T>() where T : BaseHookable => All.OfType<T>().FirstOrDefault();

	/// <summary>Registers an already constructed module. Doesn't run its lifecycle.</summary>
	public void Setup(BaseHookable hookable)
	{
		All.Add(hookable);
		HookSubscriberIndex.Invalidate();
	}

	public void Uninstall(IModule module)
	{
		All.RemoveAll(x => x == module);
		HookSubscriberIndex.Invalidate();
	}

	public void Build(params Type[] types) => Build(null, types);

	/// <summary>Instantiates every <see cref="BaseModule"/> in <paramref name="types"/> and runs it through its startup lifecycle.</summary>
	public void Build(string context, params Type[] types)
	{
		var cache = Pool.Get<List<IModule>>();

		foreach (var type in types)
		{
			if (type.IsAbstract || type.BaseType == null || !type.IsSubclassOf(typeof(BaseModule)))
			{
				continue;
			}

			var module = (BaseModule)Activator.CreateInstance(type);
			module.Context = context;

			if (module is IModule lifecycle)
			{
				cache.Add(lifecycle);
				Setup(module);
			}
		}

		// Each phase runs for every module before the next one starts, so modules can rely on each other
		Run(cache, "Init", x => x.Init());
		Run(cache, "Load", x => x.Load());
		Run(cache, "InitEnd", x => x.InitEnd(), enabledOnly: true);
		Run(cache, "OnEnableStatus", x => x.OnEnableStatus(), enabledOnly: true);

		if (Community.IsServerInitialized)
		{
			Run(cache, "OnServerInit", x => x.OnServerInit(true), enabledOnly: true);
			Run(cache, "OnPostServerInit", x => x.OnPostServerInit(true), enabledOnly: true);
		}

		Pool.FreeUnmanaged(ref cache);
	}

	public void OnServerInit()
	{
		var modules = All.OfType<IModule>().ToList();
		Run(modules, "OnServerInit", x => x.OnServerInit(true), enabledOnly: true);
		Run(modules, "OnPostServerInit", x => x.OnPostServerInit(true), enabledOnly: true);
	}

	public void OnServerSave()
	{
		Run(All.OfType<IModule>(), "OnServerSave", x => x.OnServerSaved(), enabledOnly: true);
	}

	public void Save()
	{
		Run(All.OfType<IModule>(), "Save", x => x.Save());
	}

	public void Load()
	{
		Run(All.OfType<IModule>(), "Load", x => x.Load());
	}

	public void Dispose()
	{
		Run(All.OfType<IModule>().ToList(), "Dispose", x => x.Dispose());
		All.Clear();
		HookSubscriberIndex.Invalidate();
	}

	private static void Run(IEnumerable<IModule> modules, string phase, Action<IModule> action, bool enabledOnly = false)
	{
		foreach (var module in modules)
		{
			if (enabledOnly && !module.IsEnabled())
			{
				continue;
			}

			try
			{
				action(module);
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed module {phase} for {module.GetType().FullName}", ex);
			}
		}
	}
}
