#define NO_THREADING

using System;
using System.Collections.Generic;
using System.Reflection;
using Carbon.Core;
using Carbon.Hooks;
using Carbon.Jobs;
using Facepunch;

///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Processors
{
	public class HookProcessor
	{
		public Dictionary<string, HookInstance> Patches { get; } = new Dictionary<string, HookInstance>();

		public bool DoesHookExist(string hookName)
		{
			using (TimeMeasure.New($"DoesHookExist: {hookName}"))
			{
				foreach (var hook in Defines.Hooks)
				{
					if (hook.Name == hookName) return true;
				}
			}

			return false;
		}
		public bool HasHook(Type type, string hookName)
		{
			foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (method.Name == hookName) return true;
			}

			return false;
		}
		public bool IsPatched(string hookName)
		{
			return Patches.ContainsKey(hookName);
		}
		public HookInstance GetInstance(string hookName)
		{
			if (!Patches.TryGetValue(hookName, out var instance))
			{
				return null;
			}

			return instance;
		}

		public void AppendHook(string hookName)
		{
			if (!DoesHookExist(hookName)) return;

			if (Patches.TryGetValue(hookName, out var instance))
			{
				instance.Hooks++;
			}
		}
		public void UnappendHook(string hookName)
		{
			if (!DoesHookExist(hookName)) return;

			if (Patches.TryGetValue(hookName, out var instance))
			{
				instance.Hooks--;

				if (instance.Hooks <= 0)
				{
					if (UninstallHooks(hookName))
					{
						Carbon.Logger.Warn($" No plugin is using '{hookName}'. Unpatched.");
					}
				}
			}
		}

		public void InstallHooks(string hookName, bool doRequires = true, bool onlyAlwaysPatchedHooks = false)
		{
			if (!DoesHookExist(hookName)) return;
			if (!IsPatched(hookName))
				Carbon.Logger.Debug($"Found '{hookName}'...");
#if NO_THREADING
			new HookProcessingThread
			{
				HookName = hookName,
				DoRequires = doRequires,
				Processor = this,
				OnlyAlwaysPatchedHooks = onlyAlwaysPatchedHooks
			}.ThreadFunction();
#else
			new HookProcessingThread
			{
				HookName = hookName,
				DoRequires = doRequires,
				Processor = this,
				OnlyAlwaysPatchedHooks = onlyAlwaysPatchedHooks
			}.Start();
#endif
		}
		public bool UninstallHooks(string hookName, bool shutdown = false)
		{
			try
			{
				using (TimeMeasure.New($"UninstallHooks: {hookName}"))
				{
					if (Patches.TryGetValue(hookName, out var instance))
					{
						if (!shutdown && instance.AlwaysPatched) return false;

						if (instance.Patches != null)
						{
							var list = Pool.GetList<object>();
							list.AddRange(instance.Patches);

							foreach (var patch in list)
							{
								if (patch is HarmonyLib.Harmony harmony)
								{
									if (string.IsNullOrEmpty(harmony.Id)) continue;

									harmony.UnpatchAll(harmony.Id);
								}
							}

							instance.Patches.Clear();
							Pool.FreeList(ref list);
							return true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed hook '{hookName}' uninstallation.", ex);
			}

			return false;
		}

		public void InstallAlwaysPatchedHooks()
		{
			foreach (var type in Defines.Carbon.GetTypes())
			{
				var hook = type.GetCustomAttribute<Hook>();
				if (hook == null || type.GetCustomAttribute<Hook.AlwaysPatched>() == null) continue;

				InstallHooks(hook.Name, true, true);
			}
		}

		internal Type[] GetMatchedParameters(Type type, string methodName, ParameterInfo[] parameters)
		{
			var list = new List<Type>();

			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
			{
				if (method.Name != methodName) continue;

				var @params = method.GetParameters();

				for (int i = 0; i < @params.Length; i++)
				{
					try
					{
						var param = @params[i];
						var otherParam = parameters[i];

						if (otherParam.Name.StartsWith("__")) continue;

						if (param.ParameterType.FullName.Replace("&", "") == otherParam.ParameterType.FullName.Replace("&", ""))
						{
							list.Add(param.ParameterType);
						}
					}
					catch { }
				}

				if (list.Count > 0) break;
			}

			var result = list.ToArray();
			list.Clear();
			list = null;
			return result;
		}

		public class HookInstance
		{
			public string Id { get; set; }
			public int Hooks { get; set; } = 1;
			public bool AlwaysPatched { get; set; } = false;
			public List<object> Patches { get; internal set; } = new List<object>();

			public bool success = false;
		}
	}
}
