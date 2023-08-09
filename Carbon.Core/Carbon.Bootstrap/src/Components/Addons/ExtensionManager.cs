using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Assembly;
using API.Events;
using Facepunch.Extend;
using Loaders;
using Mono.Cecil;
using UnityEngine;
using Utility;
using Logger = Utility.Logger;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class ExtensionManager : AddonManager
{
	/*
	 * CARBON EXTENSIONS
	 * API.Contracts.ICarbonExtension
	 * 
	 * An assembly to be considered as a Carbon Extension must:
	 *   1. Implement the ICarbonExtension interface
	 *   2. Must not change directly the world
	 *   3. Provide additional functionality such as new features or services
	 *
	 * Carbon extensions are different from Oxide extensions, in Carbon extensions
	 * are "libraries" and cannot access features such as hooks or change the
	 * world, either directly or using reflection.
	 *
	 */

	internal bool _hasLoaded;

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

	public class Resolver : IAssemblyResolver
	{
		internal Dictionary<string, AssemblyDefinition> _cache = new();

		public void Dispose()
		{
			_cache.Clear();
			_cache = null;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			if (!_cache.TryGetValue(name.Name, out var assembly))
			{
				var found = false;
				foreach (var directory in _references)
				{
					foreach (var file in Directory.GetFiles(directory))
					{
						switch (Path.GetExtension(file))
						{
							case ".dll":
								if (Path.GetFileNameWithoutExtension(file) == name.Name)
								{
									_cache.Add(name.Name, assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(file));
									found = true;
								}
								break;
						}

						if (found) break;
					}

					if (found) break;
				}
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
		var reloaded = false;

		Carbon.Bootstrap.Watcher.Watch(new WatchFolder
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Context.CarbonExtensions,

			OnFileCreated = (sender, file) =>
			{
				if (reloaded) return;
				reloaded = true;

				Reload("ExtensionManager.Awake");
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

					if (AssemblyManager.IsType<ICarbonExtension>(asm, out types))
					{
						Logger.Debug($"Loading extension from file '{file}'");

						var extensionTypes = new List<Type>();

						foreach (Type type in types)
						{
							try
							{
								if (Activator.CreateInstance(type) is not ICarbonExtension extension)
									throw new NullReferenceException();

								Logger.Debug($"A new instance of '{extension}' created");

								var arg = new CarbonEventArgs(file);

								extension.Awake(arg);
								extension.OnLoaded(arg);

								Carbon.Bootstrap.Events
									.Trigger(CarbonEvent.ExtensionLoaded, arg);

								extensionTypes.Add(type);
								_loaded.Add(new() { Addon = extension, Shared = asm.GetExportedTypes(), Types = extensionTypes, File = file });
							}
							catch (Exception e)
							{
								Logger.Error($"Failed to instantiate extension from type '{type}'", e);
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
			Logger.Error($"Error while loading extension from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading extension '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading extension '{file}'");

			return null;
		}
#endif
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Unload(string file, string requester)
	{

	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override void Reload(string requester)
	{
		var nonReloadables = new List<string>();

		foreach (var extension in _loaded)
		{
			if (!extension.Addon.GetType().HasAttribute(typeof(HotloadableAttribute)))
			{
				nonReloadables.Add(extension.File);
				continue;
			}

			extension.Addon.OnUnloaded(EventArgs.Empty);
		}

		var cache = new Dictionary<string, AssemblyDefinition>();
		var streams = new List<MemoryStream>();
		var extensions = new Dictionary<string, ICarbonExtension>();

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
				if (nonReloadables.Contains(file)) continue;

				switch (Path.GetExtension(file))
				{
					case ".dll":
						if (!_hasLoaded)
						{
							Load(file, "ExtensionManager.Reload");
							continue;
						}

						var stream = new MemoryStream(Process(File.ReadAllBytes(file)));
						var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(stream, new ReaderParameters { AssemblyResolver = new Resolver() });
						var originalName = assembly.Name.Name;
						assembly.Name = new AssemblyNameDefinition($"{assembly.Name.Name}_{Guid.NewGuid()}", assembly.Name.Version);
						cache.Add(originalName, assembly);
						break;
				}
			}
		}

		_hasLoaded = true;

		nonReloadables.Clear();
		nonReloadables = null;

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

			if (AssemblyManager.IsType<ICarbonExtension>(processedAssembly, out var types))
			{
				var file = Path.Combine(Context.CarbonExtensions, $"{_assembly.Key}.dll");
				var existentItem = _loaded.FirstOrDefault(x => x.File == file);
				if (existentItem == null)
				{
					_loaded.Add(existentItem = new() { File = file });
				}

				existentItem.Shared = processedAssembly.GetExportedTypes();

				var extensionTypes = new List<Type>();
				foreach (var type in types)
				{
					if (Activator.CreateInstance(type) is ICarbonExtension ext)
					{
						extensionTypes.Add(type);
						existentItem.Addon = ext;

						Logger.Debug($"A new instance of '{type}' created");
						extensions.Add(_assembly.Key, ext);
					}
				}
				existentItem.Types = extensionTypes;
			}
		}

		foreach (var extension in extensions)
		{
			try
			{
				var file = Path.Combine(Context.CarbonExtensions, $"{extension.Key}.dll");
				var arg = new CarbonEventArgs(file);

				extension.Value.Awake(arg);
				extension.Value.OnLoaded(arg);

				Carbon.Bootstrap.Events
					.Trigger(CarbonEvent.ExtensionLoaded, arg);
			}
			catch (Exception e)
			{
				Logger.Error($"Failed to instantiate extension from type '{extension.Value}'\n{e}\nInner: {e.InnerException}");
				continue;
			}
		}

		_hasLoaded = true;

		Dispose();

		void Dispose()
		{
			foreach(var stream in streams)
			{
				stream.Dispose();
			}

			extensions.Clear();
			streams.Clear();
			cache.Clear();

			extensions = null;
			streams = null;
			cache = null;
		}
	}
}
