using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon;
using Carbon.Base;
using Carbon.Components;
using Carbon.Contracts;
using Carbon.Core;
using Facepunch;
using Newtonsoft.Json;
using Oxide.Core.Configuration;
using Oxide.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Plugins
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Plugin : BaseHookable, IDisposable
	{
		public PluginManager Manager { get; set; }

		public bool IsCorePlugin { get; set; }

		[JsonProperty]
		public string Title { get; set; } = "Rust";
		[JsonProperty]
		public string Description { get; set; }
		[JsonProperty]
		public string Author { get; set; }
		public int ResourceId { get; set; }
		public bool HasConfig { get; set; }
		public bool HasMessages { get; set; }
		public bool HasConditionals { get; set; }

		[JsonProperty]
		public double CompileTime { get; set; }
		[JsonProperty]
		public ModLoader.FailedMod.Error[] CompileWarnings { get; set; }

		[JsonProperty]
		public string FilePath { get; set; }
		[JsonProperty]
		public string FileName { get; set; }

		public string Filename => FileName;

		public override void TrackStart()
		{
			if (IsCorePlugin) return;

			base.TrackStart();
		}
		public override void TrackEnd()
		{
			if (IsCorePlugin) return;

			base.TrackEnd();
		}

		public Plugin[] Requires { get; set; }

		public ModLoader.ModPackage Package;
		public IBaseProcessor Processor;
		public IBaseProcessor.IInstance ProcessorInstance;

		public static implicit operator bool(Plugin other)
		{
			return other != null && other.IsLoaded;
		}

		public virtual void IInit()
		{
			if (HookMethods != null)
			{
				using (TimeMeasure.New($"Processing HookMethods on '{this}'"))
				{
					foreach (var attribute in HookMethods)
					{
						var method = attribute.Method;
						var priority = method.GetCustomAttribute<HookPriority>();

						var hash = (uint)(HookCallerCommon.StringPool.GetOrAdd(string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name) + method.GetParameters().Length);
						if (!HookMethodAttributeCache.TryGetValue(hash, out var list))
						{
							HookMethodAttributeCache.Add(hash, new() { CachedHook.Make(method, priority == null ? Priorities.Normal : priority.Priority, this) });
						}
						else list.Add(CachedHook.Make(method, priority == null ? Priorities.Normal : priority.Priority, this));
					}
				}
				Carbon.Logger.Debug(Name, "Installed hook method attributes");
			}

			using (TimeMeasure.New($"Processing PluginReferences on '{this}'"))
			{
				InternalApplyPluginReferences();
			}
			Carbon.Logger.Debug(Name, "Assigned plugin references");

			if (Hooks != null)
			{
				string requester = FileName is not default(string) ? FileName : $"{this}";
				using (TimeMeasure.New($"Processing Hooks on '{this}'"))
				{
					foreach (var hook in Hooks)
					{
						Community.Runtime.HookManager.Subscribe(HookCallerCommon.StringPool.GetOrAdd(hook.Key), requester);
					}
						
				}
				Carbon.Logger.Debug(Name, "Processed hooks");
			}

			CallHook("Init");
		}
		public virtual void Load()
		{
			using (TimeMeasure.New($"Load on '{this}'"))
			{
				IsLoaded = true;
				CallHook("OnLoaded");
				CallHook("Loaded");
			}

			using (TimeMeasure.New($"Load.PendingRequirees on '{this}'"))
			{
				var requirees = ModLoader.GetRequirees(this);

				if (requirees != null)
				{
					foreach (var requiree in requirees)
					{
						Logger.Warn($" [{Name}] Loading '{Path.GetFileNameWithoutExtension(requiree)}' to parent's request: '{ToString()}'");
						Community.Runtime.ScriptProcessor.Prepare(requiree);
					}

					ModLoader.ClearPendingRequirees(this);
				}
			}
		}
		public virtual void IUnload()
		{
			try
			{
				using (TimeMeasure.New($"IUnload.UnprocessHooks on '{this}'"))
				{
					foreach (var hook in Hooks)
					{
						Community.Runtime.HookManager.Unsubscribe(HookCallerCommon.StringPool.GetOrAdd(hook.Key), FileName);
					}
					Carbon.Logger.Debug(Name, $"Unprocessed hooks");
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed calling Plugin.IUnload.UnprocessHooks on {this}", ex);
			}

			try
			{
				using (TimeMeasure.New($"IUnload.Disposal on '{this}'"))
				{
					IgnoredHooks?.Clear();
					HookCache?.Clear();
					Hooks?.Clear();
					HookMethods?.Clear();
					PluginReferences?.Clear();
					HookMethodAttributeCache?.Clear();

					IgnoredHooks = null;
					HookCache = null;
					Hooks = null;
					HookMethods = null;
					PluginReferences = null;
					HookMethodAttributeCache = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed calling Plugin.IUnload.Disposal on {this}", ex);
			}

			try
			{
				using (TimeMeasure.New($"IUnload.UnloadRequirees on '{this}'"))
				{
					var mods = Pool.GetList<ModLoader.ModPackage>();
					mods.AddRange(ModLoader.LoadedPackages);
					var plugins = Pool.GetList<Plugin>();

					foreach (var mod in ModLoader.LoadedPackages)
					{
						plugins.Clear();
						plugins.AddRange(mod.Plugins);

						foreach (var plugin in plugins)
						{
							if (plugin.Requires != null && plugin.Requires.Contains(this))
							{
								switch (plugin.Processor)
								{
									case IScriptProcessor script:
										Logger.Warn($" [{Name}] Unloading '{plugin.ToString()}' because parent '{ToString()}' has been unloaded.");
										ModLoader.AddPendingRequiree(this, plugin);
										plugin.Processor.Get<IScriptProcessor.IScript>(plugin.FileName)?.Dispose();
										break;
								}
							}
						}
					}

					Pool.FreeList(ref mods);
					Pool.FreeList(ref plugins);
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed calling Plugin.IUnload.UnloadRequirees on {this}", ex);
			}
		}
		internal void InternalApplyPluginReferences()
		{
			if (PluginReferences == null) return;

			foreach (var reference in PluginReferences)
			{
				var field = reference.Field;
				var attribute = field.GetCustomAttribute<PluginReferenceAttribute>();
				if (attribute == null) continue;

				var name = string.IsNullOrEmpty(attribute.Name) ? field.Name : attribute.Name;

				var plugin = (Plugin)null;
				if (field.FieldType.Name != nameof(Plugin) && field.FieldType.Name != nameof(RustPlugin))
				{
					var info = field.FieldType.GetCustomAttribute<InfoAttribute>();
					if (info == null)
					{
						Carbon.Logger.Warn($"You're trying to reference a non-plugin instance: {name}[{field.FieldType.Name}]");
						continue;
					}

					plugin = Community.Runtime.CorePlugin.plugins.Find(info.Title);
				}
				else
				{
					plugin = Community.Runtime.CorePlugin.plugins.Find(name);
				}

				if (plugin != null) field.SetValue(this, plugin);
			}
		}

		public static void InternalApplyAllPluginReferences()
		{
			foreach (var package in ModLoader.LoadedPackages)
			{
				foreach (var plugin in package.Plugins)
				{
					plugin.InternalApplyPluginReferences();
				}
			}
		}

		public void SetProcessor(IBaseProcessor processor)
		{
			Processor = processor;
		}

		#region Calls

		public T Call<T>(string hook)
		{
			return HookCaller.CallHook<T>(this, hook);
		}
		public T Call<T>(string hook, object arg1)
		{
			return HookCaller.CallHook<T>(this, hook, arg1);
		}
		public T Call<T>(string hook, object arg1, object arg2)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
		public T Call<T>(string hook, object[] args)
		{
			return args.Length switch
			{
				1 => HookCaller.CallHook<T>(this, hook, args[0]),
				2 => HookCaller.CallHook<T>(this, hook, args[0], args[1]),
				3 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2]),
				4 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3]),
				5 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4]),
				6 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5]),
				7 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
				8 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
				9 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
				10 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
				11 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
				12 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
				13 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[13]),
				_ => HookCaller.CallHook<T>(this, hook),
			};
		}

		public object Call(string hook)
		{
			return HookCaller.CallHook(this, hook);
		}
		public object Call(string hook, object arg1)
		{
			return HookCaller.CallHook(this, hook, arg1);
		}
		public object Call(string hook, object arg1, object arg2)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2);
		}
		public object Call(string hook, object arg1, object arg2, object arg3)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}
		public object Call(string hook, object[] args)
		{
			return args?.Length switch
			{
				1 => HookCaller.CallHook(this, hook, args[0]),
				2 => HookCaller.CallHook(this, hook, args[0], args[1]),
				3 => HookCaller.CallHook(this, hook, args[0], args[1], args[2]),
				4 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3]),
				5 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4]),
				6 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5]),
				7 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
				8 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
				9 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
				10 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
				11 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
				12 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
				13 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
				_ => HookCaller.CallHook(this, hook),
			};
		}

		public T CallHook<T>(string hook)
		{
			return HookCaller.CallHook<T>(this, hook);
		}
		public T CallHook<T>(string hook, object arg1)
		{
			return HookCaller.CallHook<T>(this, hook, arg1);
		}
		public T CallHook<T>(string hook, object arg1, object arg2)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
		{
			return HookCaller.CallHook<T>(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}
		public T CallHook<T>(string hook, object[] args)
		{
			return args.Length switch
			{
				1 => HookCaller.CallHook<T>(this, hook, args[0]),
				2 => HookCaller.CallHook<T>(this, hook, args[0], args[1]),
				3 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2]),
				4 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3]),
				5 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4]),
				6 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5]),
				7 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
				8 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
				9 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
				10 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
				11 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
				12 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
				13 => HookCaller.CallHook<T>(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[13]),
				_ => HookCaller.CallHook<T>(this, hook),
			};
		}

		public object CallHook(string hook)
		{
			return HookCaller.CallHook(this, hook);
		}
		public object CallHook(string hook, object arg1)
		{
			return HookCaller.CallHook(this, hook, arg1);
		}
		public object CallHook(string hook, object arg1, object arg2)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
		{
			return HookCaller.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}
		public object CallHook(string hook, object[] args)
		{
			return args?.Length switch
			{
				1 => HookCaller.CallHook(this, hook, args[0]),
				2 => HookCaller.CallHook(this, hook, args[0], args[1]),
				3 => HookCaller.CallHook(this, hook, args[0], args[1], args[2]),
				4 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3]),
				5 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4]),
				6 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5]),
				7 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
				8 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
				9 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
				10 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
				11 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
				12 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
				13 => HookCaller.CallHook(this, hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
				_ => HookCaller.CallHook(this, hook),
			};
		}

		#endregion

		#region Compatibility

		public virtual void HandleAddedToManager(PluginManager manager) { }
		public virtual void HandleRemovedFromManager(PluginManager manager) { }

		#endregion

		public void NextTick(Action callback)
		{
			Community.Runtime.CarbonProcessor.OnFrameQueue.Enqueue(callback);
		}
		public void NextFrame(Action callback)
		{
			Community.Runtime.CarbonProcessor.OnFrameQueue.Enqueue(callback);
		}

		public DynamicConfigFile Config { get; internal set; }

		public bool IsLoaded { get; set; }

		protected virtual void LoadConfig()
		{
			Config = new DynamicConfigFile(Path.Combine(Manager.ConfigPath, Name + ".json"));

			if (!Config.Exists(null))
			{
				CallHook("LoadDefaultConfig");

				SaveConfig();
			}
			try
			{
				if (Config.Exists(null)) Config.Load(null);
			}
			catch (Exception ex)
			{
				Carbon.Logger.Error("Failed to load config file (is the config file corrupt?) (" + ex.Message + ")");
			}
		}
		protected virtual void LoadDefaultConfig()
		{
			//CallHook ( "LoadDefaultConfig" );
		}
		protected virtual void SaveConfig()
		{
			if (Config == null)
			{
				return;
			}
			try
			{
				if (Config.Count() > 0) Config.Save(null);
			}
			catch (Exception ex)
			{
				Carbon.Logger.Error("Failed to save config file (does the config have illegal objects in it?) (" + ex.Message + ")", ex);
			}
		}

		protected virtual void LoadDefaultMessages()
		{

		}

		public new string ToString()
		{
			return GetType().Name;
		}
		public virtual void Dispose()
		{
			IsLoaded = false;
		}
	}
}
