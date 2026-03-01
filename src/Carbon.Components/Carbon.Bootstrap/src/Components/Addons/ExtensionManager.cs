using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Events;
using Carbon;
using Carbon.Components;
using Carbon.Extensions;
using Carbon.Profiler;
using Facepunch;
using Facepunch.Extend;
using Loaders;
using Mono.Cecil;
using Steamworks.Data;
using UnityEngine;
using Utility;
using Logger = Utility.Logger;

namespace Components;

#pragma warning disable IDE0051

internal sealed class ExtensionManager : AddonManager, IExtensionManager
{
	private readonly string[] _directories =
	{
		Context.CarbonExtensions,
	};
	private static readonly string[] _references =
	{
		Context.CarbonExtensions,
		Context.CarbonManaged,
		Context.CarbonLib,
		Context.GameManaged
	};

	public static Dictionary<string, Assembly> ExtensionAssemblyCache = new();
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
					switch (Path.GetExtension(file))
					{
						case ".dll":
							if (Path.GetFileNameWithoutExtension(file) == name.Name)
							{
								Cache.Add(name.Name, assembly = AssemblyDefinition.ReadAssembly(file, ReadingParameters));
								found = true;
							}
							break;
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

	internal List<string> _created = [];
	internal List<string> _changed = [];
	internal List<string> _deleted = [];

	internal void Awake()
	{
		Carbon.Bootstrap.Watcher.Watch(Watcher = new WatchFolder
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Context.CarbonExtensions,

			OnFileCreated = (_, file) =>
			{
				if (!Watcher.InitialEvent)
				{
					return;
				}

				if (!_created.Contains(file) && !_changed.Contains(file) && !_deleted.Contains(file))
				{
					_created.Add(file);
				}
			},
		});

		Watcher.Handler.EnableRaisingEvents = false;
	}

	internal void FixedUpdate()
	{
		foreach (var file in _created)
		{
			try
			{
				Load(file, "ExtensionManager.Created");
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		foreach (var file in _changed)
		{
			try
			{
				Unload(file, "ExtensionManager.Changed");
				Load(file, "ExtensionManager.Changed");
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		foreach (var file in _deleted)
		{
			try
			{
				Unload(file, "ExtensionManager.Deleted");
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		_created.Clear();
		_changed.Clear();
		_deleted.Clear();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly Load(string file, string requester = null)
	{
		if (requester is null)
		{
			MethodBase caller = new StackFrame(1).GetMethod();
			requester = $"{caller.DeclaringType}.{caller.Name}";
		}

		var item = _loaded.FirstOrDefault(x => x.File == file);
		var definition = (AssemblyDefinition)null;
		var stream = (MemoryStream)null;
		var extension = (ICarbonExtension)null;
		var assemblyName = string.Empty;
		var result = (Assembly)null;

		if (File.Exists(file))
		{
			switch (Path.GetExtension(file))
			{
				case ".dll":
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
					break;
			}
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
		result = _loader.Load(file, requester, _directories, IExtensionManager.ExtensionTypes.Extension)?.Assembly;

		ExtensionAssemblyCache[result.FullName] = result;

		var isProfiled = MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Extension, result, Path.GetFileNameWithoutExtension(file));
		Assemblies.Extensions.Update(Path.GetFileNameWithoutExtension(file), result, file, isProfiled);

		if (AssemblyManager.IsType<ICarbonExtension>(result, out var types))
		{
			var moduleFile = Path.Combine(Context.CarbonExtensions, $"{assemblyName}.dll");

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
					if (!type.GetInterfaces().Contains(typeof(ICarbonExtension)))
					{
						continue;
					}

					extension = Activator.CreateInstance(type) as ICarbonExtension;

					Hydrate(result, extension);

					moduleTypes.Add(type);
					item.Addon = extension;
				}
			}

			item.Types = moduleTypes;
		}

		if (extension == null)
		{
			Logger.Error($"Failed loading extension '{file}'");

			Dispose();
			return null;
		}

		var arg = Pool.Get<CarbonEventArgs>();
		arg.Init(file);

		try
		{
			extension.Awake(arg);
			extension.OnLoaded(arg);

			Carbon.Bootstrap.Events.Trigger(CarbonEvent.ExtensionLoaded, arg);

		}
		catch (Exception e)
		{
			Logger.Error($"Failed to instantiate module from type '{assemblyName}' [{file}]", e);

			Carbon.Bootstrap.Events.Trigger(CarbonEvent.ExtensionLoadFailed, arg);
		}

		Pool.Free(ref arg);

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
		var arg = Pool.Get<CarbonEventArgs>();
		arg.Init(file);

		try
		{
			Carbon.Bootstrap.Events.Trigger(CarbonEvent.ExtensionUnloaded, arg);

			item.Addon.OnUnloaded(EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed unloading extension '{file}' (requested by {requester})", ex);

			Carbon.Bootstrap.Events.Trigger(CarbonEvent.ExtensionUnloadFailed, arg);
		}

		Pool.Free(ref arg);

		_loaded.Remove(item);
	}
}
