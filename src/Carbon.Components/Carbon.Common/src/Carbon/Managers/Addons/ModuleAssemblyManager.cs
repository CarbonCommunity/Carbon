using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Carbon.Events;
using Carbon.Components;
using Carbon.Extensions;
using Carbon.Pooling;
using Carbon.Profiler;
using Facepunch;
using Facepunch.Extend;
using Mono.Cecil;

namespace Carbon.Managers;
#pragma warning disable IDE0051

/// <summary>
/// Loads module assemblies (IModulePackage) from carbon/managed/modules, hot-reloading them on change.
/// </summary>
public class ModuleAssemblyManager : AddonManager
{
	private static readonly string[] _references =
	{
		Defines.GetManagedModulesFolder(),
		Defines.GetExtensionsFolder(),
		Defines.GetManagedFolder(),
		Defines.GetLibFolder(),
		Defines.GetRustManagedFolder()
	};

	public static Dictionary<string, Assembly> ModuleAssemblyCache = new();
	public static Resolver ResolverInstance;
	public static ReaderParameters ReadingParameters = new() { AssemblyResolver = ResolverInstance = new Resolver()};

	public class Resolver : IAssemblyResolver
	{
		internal Dictionary<string, AssemblyDefinition> Cache = new();

		public void Dispose()
		{
			Cache.Clear();
			Cache = null;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			if (Cache.TryGetValue(name.Name, out var assembly))
			{
				return assembly;
			}

			var found = false;
			foreach(var directory in _references)
			{
				foreach(var file in Directory.GetFiles(directory))
				{
					if (PathEx.HasExtension(file, ".dll") &&
						Path.GetFileNameWithoutExtension(file) == name.Name)
					{
						Cache.Add(name.Name, assembly = AssemblyDefinition.ReadAssembly(file, ReadingParameters));
						found = true;
					}

					if (found) break;
				}

				if (found) break;
			}

			return assembly;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			return Resolve(name);
		}
	}

	internal void Awake()
	{
		Services.FileWatcher.Watch(Watcher = new WatchFolder
		{
			Filter = "*.dll",
			IncludeSubFolders = false,
			Directory = Defines.GetManagedModulesFolder(),

			OnEvent = e =>
			{
				if (!e.IsInitial) return;
				if (e.Type != WatcherChangeTypes.Created) return;

				Load(e.Path, "ModuleManager.Created");
			},
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly Load(string file, string requester = null)
	{
		var item = _loaded.FirstOrDefault(x => x.File == file);
		var definition = (AssemblyDefinition)null;
		var stream = (MemoryStream)null;
		var module = (IModulePackage)null;
		var assemblyName = string.Empty;
		var result = (Assembly)null;

		if (File.Exists(file) && PathEx.HasExtension(file, ".dll"))
		{
			stream = new MemoryStream(File.ReadAllBytes(file));

			var assembly = AssemblyDefinition.ReadAssembly(stream, ReadingParameters);
			assemblyName = assembly.Name.Name;

			assembly.Name.Name = $"{assembly.Name.Name}_{Guid.NewGuid()}";

			foreach (var reference in assembly.MainModule.AssemblyReferences)
			{
				if (ResolverInstance.Cache.TryGetValue(reference.Name, out var assemblyDefinition))
				{
					reference.Name = assemblyDefinition.Name.Name;
				}
			}

			ResolverInstance.Cache[assemblyName] = assembly;

			definition = assembly;
		}

		if (definition == null || string.IsNullOrEmpty(assemblyName))
		{
			Dispose();
			return null;
		}

		using MemoryStream memoryStream = new MemoryStream();
		definition.Write(memoryStream);
		memoryStream.Position = 0;
		definition.Dispose();

		var bytes = memoryStream.ToArray();
		result = Assembly.Load(bytes);
		ModuleAssemblyCache[result.FullName] = result;

		var isProfiled = MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Module, result, Path.GetFileNameWithoutExtension(file));
		Assemblies.Modules.Update(Path.GetFileNameWithoutExtension(file), result, file, isProfiled);

		if (AssemblyManager.IsType<IModulePackage>(result, out var types))
		{
			var moduleFile = Path.Combine(Defines.GetManagedModulesFolder(), $"{assemblyName}.dll");

			if (item == null)
			{
				_loaded.Add(item = new() { File = moduleFile });
			}

			item.PostProcessedRaw = bytes;
			item.Shared = result.GetTypes();

			var moduleTypes = new List<Type>();

			if (types != null)
			{
				foreach (var type in types)
				{
					if (!typeof(IModulePackage).IsAssignableFrom(type)) continue;

					module = Activator.CreateInstance(type) as IModulePackage;


					moduleTypes.Add(type);
					item.Addon = module;
				}
			}

			item.Types = moduleTypes;
		}

		if (module == null)
		{
			Logger.Error($"Failed loading module '{file}'");

			Dispose();
			return null;
		}

		try
		{
			var arg = Pool.Get<CarbonEventArgs>();
			arg.Init(file);

			module.Awake(arg);
			module.OnLoaded(arg);

			Pool.Free(ref arg);

			var arg2 = Pool.Get<ModuleEventArgs>();
			arg2.Init(file, module, item.Shared);
			Services.Events.Trigger(CarbonEvent.ModuleLoaded, arg2);
			Pool.Free(ref arg2);
		}
		catch (Exception e)
		{
			Logger.Error($"Failed to instantiate module from type '{assemblyName}' [{file}]", e);

			var arg2 = Pool.Get<ModuleEventArgs>();
			arg2.Init(file, module, item.Shared);
			Services.Events.Trigger(CarbonEvent.ModuleLoadFailed, arg2);
			Pool.Free(ref arg2);
		}

		Dispose();

		void Dispose()
		{
			stream?.Dispose();
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Unload(string file, string requester)
	{
		var item = _loaded.FirstOrDefault(x => x.File == file);

		if (item == null)
		{
			Logger.Log($"Couldn't find module '{file}' (requested by {requester})");
			return;
		}

		try
		{
			var arg = Pool.Get<ModuleEventArgs>();
			arg.Init(file, (IModulePackage)item.Addon, null);
			Services.Events.Trigger(CarbonEvent.ModuleUnloaded, arg);
			Pool.Free(ref arg);

			item.Addon.OnUnloaded(EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed unloading module '{file}' (requested by {requester})", ex);

			var arg = Pool.Get<ModuleEventArgs>();
			arg.Init(file, (IModulePackage)item.Addon, null);
			Services.Events.Trigger(CarbonEvent.ModuleUnloadFailed, arg);
			Pool.Free(ref arg);
		}

		_loaded.Remove(item);
	}
}
