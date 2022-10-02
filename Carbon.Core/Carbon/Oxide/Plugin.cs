///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbon.Core;
using Carbon.Core.Processors;
using Harmony;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Plugin : BaseHookable, IDisposable
	{
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

		[JsonProperty]
		public double CompileTime { get; internal set; }

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

		public CarbonLoader.CarbonMod carbon { get; set; }

		public Plugin[] Requires { get; internal set; }

		internal BaseProcessor _processor;

		public HarmonyInstance Harmony;

		public static implicit operator bool(Plugin other)
		{
			return other != null;
		}

		internal void _processHooks()
		{
			foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				CarbonCore.Instance.Addon.InstallHooks(method.Name);
				CarbonCore.Instance.Addon.AppendHook(method.Name);
			}

			CarbonCore.Debug(Name, "Processed hooks", 2);
		}
		internal void _unprocessHooks()
		{
			foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				CarbonCore.Instance.Addon.UnappendHook(method.Name);
			}

			CarbonCore.Debug(Name, "Unprocessed hooks", 2);
		}

		public virtual void IInit()
		{
			foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{
				var attribute = method.GetCustomAttribute<HookMethodAttribute>();
				if (attribute == null) continue;

				var name = (string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name) + method.GetParameters().Length;
				if (!HookMethodAttributeCache.TryGetValue(name, out var list))
				{
					HookMethodAttributeCache.Add(name, new List<MethodInfo>() { method });
				}
				else list.Add(method);
			}

			CarbonCore.Debug(Name, "Installed hook method attributes", 2);

			foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{
				var attribute = field.GetCustomAttribute<PluginReferenceAttribute>();
				if (attribute == null) continue;

				var plugin = (Plugin)null;
				if (field.FieldType != typeof(Plugin) && field.FieldType != typeof(RustPlugin))
				{
					var info = field.FieldType.GetCustomAttribute<InfoAttribute>();
					if (info == null)
					{
						CarbonCore.Error($"You're trying to reference a non-plugin instance: {field.Name}[{field.FieldType.Name}]");
						continue;
					}

					plugin = CarbonCore.Instance.CorePlugin.plugins.Find(info.Title);
				}
				else plugin = CarbonCore.Instance.CorePlugin.plugins.Find(field.Name);

				if (plugin != null) field.SetValue(this, plugin);
			}

			CarbonCore.Debug(Name, "Assigned plugin references", 2);

			_processHooks();

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
		}
		public virtual void IUnload()
		{
			using (TimeMeasure.New($"IUnload._unprocessHooks on '{this}'"))
			{
				_unprocessHooks();
			}

			using (TimeMeasure.New($"IUnload.requires on '{this}'"))
			{
				foreach (var plugin in CarbonCore.Instance.CorePlugin.plugins.GetAll())
				{
					if (plugin == null || plugin.Requires == null || plugin.Requires.Length == 0) continue;

					if (plugin.Requires.Contains(this))
					{
						plugin.InternalUnload();
					}
				}
			}

			using (TimeMeasure.New($"IUnload.disposal on '{this}'"))
			{
				IgnoredHooks.Clear();
				HookCache.Clear();
				HookMethodAttributeCache.Clear();

				IgnoredHooks = null;
				HookCache = null;
				HookMethodAttributeCache = null;
			}
		}
		internal void InternalUnload()
		{
			// switch ( _processor )
			// {
			//     case ScriptProcessor script:
			//         if ( script.InstanceBuffer.TryGetValue ( Name, out var instance ) )
			//             _processor.Clear ( Name, instance );
			//         break;
			// }
		}

		public void PatchPlugin(Assembly assembly = null)
		{
			if (assembly == null) assembly = Assembly.GetExecutingAssembly();

			Harmony = null;
			Harmony = HarmonyInstance.Create(Name + "Patches");
			Harmony.PatchAll(assembly);
		}
		public void UnpatchPlugin()
		{
			Harmony?.UnpatchAll(Harmony.Id);
			Harmony = null;
		}

		public void SetProcessor(BaseProcessor processor)
		{
			_processor = processor;
		}

		#region Calls

		public T Call<T>(string hook)
		{
			return (T)HookExecutor.CallHook(this, hook);
		}
		public T Call<T>(string hook, object arg1)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1);
		}
		public T Call<T>(string hook, object arg1, object arg2)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public T Call<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public object Call(string hook)
		{
			return HookExecutor.CallHook(this, hook);
		}
		public object Call(string hook, object arg1)
		{
			return HookExecutor.CallHook(this, hook, arg1);
		}
		public object Call(string hook, object arg1, object arg2)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2);
		}
		public object Call(string hook, object arg1, object arg2, object arg3)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public object Call(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public T CallHook<T>(string hook)
		{
			return (T)HookExecutor.CallHook(this, hook);
		}
		public T CallHook<T>(string hook, object arg1)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1);
		}
		public T CallHook<T>(string hook, object arg1, object arg2)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public T CallHook<T>(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return (T)HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public object CallHook(string hook)
		{
			return HookExecutor.CallHook(this, hook);
		}
		public object CallHook(string hook, object arg1)
		{
			return HookExecutor.CallHook(this, hook, arg1);
		}
		public object CallHook(string hook, object arg1, object arg2)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookExecutor.CallHook(this, hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		#endregion

		public void NextTick(Action callback)
		{
			CarbonCore.Instance.CarbonProcessor.OnFrameQueue.Enqueue(callback);
		}
		public void NextFrame(Action callback)
		{
			CarbonCore.Instance.CarbonProcessor.OnFrameQueue.Enqueue(callback);
		}

		public bool IsLoaded { get; set; }

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
