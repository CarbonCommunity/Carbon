using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Events;
using Loaders;
using Mono.Cecil;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ModuleManager : AddonManager
{
	/*
	 * CARBON MODULES
	 * API.Contracts.ICarbonModule
	 * 
	 * An assembly to be considered as a Carbon Module must:
	 *   1. Be optional
	 *   2. Implement the ICarbonModule interface
	 *   3. Provide additional functionality such as new features or services
	 *
	 * Carbon modules can be compared to Oxide Extensions, they can be created
	 * by anyone and can change and/or interact with the world as any other user
	 * plugin can.
	 *
	 */
	private readonly string[] _directories =
	{
		Context.CarbonModules,
	};

	internal void Awake()
	{
		var reloaded = false;

		Carbon.Bootstrap.Watcher.Watch(new WatchFolder
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Context.CarbonModules,

			OnFileCreated = (sender, file) =>
			{
				if (reloaded) return;
				reloaded = true;

				Reload("ModuleManager.Awake");
			},
		});
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly Load(string file, string requester = null)
	{
		if (requester is null)
		{
			MethodBase caller = new StackFrame(1).GetMethod();
			requester = $"{caller.DeclaringType}.{caller.Name}";
		}

		IReadOnlyList<string> blacklist = AssemblyManager.RefBlacklist;
		IReadOnlyList<string> whitelist = null;

		try
		{
			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					Assembly asm = _loader.Load(file, requester, _directories, blacklist, whitelist)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);

					if (AssemblyManager.IsType<ICarbonModule>(asm, out types))
					{
						Logger.Debug($"Loading module from file '{file}'");

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonModule module)
									throw new NullReferenceException();

								Logger.Debug($"A new instance of '{module}' created");
								Hydrate(asm, module);

								module.Awake(EventArgs.Empty);
								module.OnLoaded(EventArgs.Empty);

								// for now force all modules to be enabled when loaded
								module.OnEnable(EventArgs.Empty);

								Carbon.Bootstrap.Events
									.Trigger(CarbonEvent.ModuleLoaded, new CarbonEventArgs(file));

								_loaded.Add(new() { Addon = module, Types = asm.GetExportedTypes(), File = file });
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate module from type '{type}'", e);
								continue;
							}
						}
					}
					else
					{
						throw new Exception("Unsupported assembly type");
					}

					return asm;

				// case ".drm"
				// 	LoadFromDRM();
				// 	break;

				default:
					throw new Exception("File extension not supported");
			}
		}
		catch (ReflectionTypeLoadException)
		{
			Logger.Error($"Error while loading module from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading module '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading module '{file}'");

			return null;
		}
#endif
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
			item.Addon.OnUnloaded(EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed unloading module '{file}' (requested by {requester})", ex);
		}

		_loaded.Remove(item);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Reload(string requester)
	{
		foreach (var module in _loaded)
		{
			module.Addon.OnUnloaded(EventArgs.Empty);
		}

		_loaded.Clear();

		var cache = new Dictionary<string, AssemblyDefinition>();
		var streams = new List<MemoryStream>();
		var modules = new Dictionary<string, ICarbonModule>();

		static byte[] Process(byte[] raw)
		{
			if (AssemblyLoader.IndexOf(raw, new byte[4] { 0x01, 0xdc, 0x7f, 0x01 }) == 0)
			{
				byte[] checksum = new byte[20];
				Buffer.BlockCopy(raw, 4, checksum, 0, 20);
				return AssemblyLoader.Package(checksum, raw, 24);
			}

			return raw;
		}

		foreach (var directory in _directories)
		{
			foreach (var file in Directory.GetFiles(directory))
			{
				switch (Path.GetExtension(file))
				{
					case ".dll":
						var stream = new MemoryStream(Process(File.ReadAllBytes(file)));
						var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(stream);
						var originalName = assembly.Name.Name;
						assembly.Name = new AssemblyNameDefinition($"{assembly.Name.Name}_{Guid.NewGuid()}", assembly.Name.Version);
						cache.Add(originalName, assembly);
						break;
				}
			}
		}

		foreach (var _assembly in cache)
		{
			foreach (var refer in _assembly.Value.MainModule.AssemblyReferences)
			{
				if (cache.TryGetValue(refer.Name, out var assembly))
				{
					refer.Name = assembly.Name.Name;
				}
			}

			using MemoryStream memoryStream = new MemoryStream();
			_assembly.Value.Write(memoryStream);
			memoryStream.Position = 0;
			_assembly.Value.Dispose();

			var bytes = memoryStream.ToArray();
			var processedAssembly = Assembly.Load(bytes);
			Array.Clear(bytes, 0, bytes.Length);

			if (AssemblyManager.IsType<ICarbonModule>(processedAssembly, out var types))
			{
				foreach (var type in types)
				{
					if (Activator.CreateInstance(type) is ICarbonModule mod)
					{
						Logger.Debug($"A new instance of '{type}' created");
						modules.Add(_assembly.Key, mod);
					}
				}
			}
		}

		foreach (var module in modules)
		{
			try
			{
				var file = Path.Combine(Context.CarbonModules, $"{module.Key}.dll");
				var arg = new CarbonEventArgs(file);

				module.Value.Awake(arg);
				module.Value.OnLoaded(arg);

				Carbon.Bootstrap.Events
					.Trigger(CarbonEvent.ModuleLoaded, arg);

				_loaded.Add(new() { Addon = module.Value, File = file });
			}
			catch (Exception e)
			{
				Logger.Error($"Failed to instantiate module from type '{module.Value}'", e);
				continue;
			}
		}

		Dispose();

		void Dispose()
		{
			foreach (var stream in streams)
			{
				stream.Dispose();
			}

			modules.Clear();
			modules = null;
			streams.Clear();
			streams = null;
			cache.Clear();
			cache = null;
		}
	}

	internal override void Hydrate(Assembly assembly, ICarbonAddon addon)
	{
		base.Hydrate(assembly, addon);

		BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		Type logger = typeof(API.Logger.ILogger) ?? throw new Exception();
		Type events = typeof(API.Events.IEventManager) ?? throw new Exception();

		foreach (Type type in assembly.GetTypes())
		{
			foreach (FieldInfo item in type.GetFields(flags)
				.Where(x => logger.IsAssignableFrom(x.FieldType)))
			{
				item.SetValue(assembly,
					Activator.CreateInstance(HarmonyLib.AccessTools.TypeByName("Carbon.Logger") ?? null));
			}

			foreach (FieldInfo item in type.GetFields(flags)
				.Where(x => events.IsAssignableFrom(x.FieldType)))
			{
				item.SetValue(assembly, Carbon.Bootstrap.Events);
			}
		}
	}
}
