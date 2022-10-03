///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Facepunch;
using Harmony;

namespace Carbon.Core
{
	public class CarbonAddonProcessor
	{
		public List<Assembly> Addons { get; } = new List<Assembly>();
		public Dictionary<string, HookInstance> Patches { get; } = new Dictionary<string, HookInstance>();

		public CarbonAddonProcessor()
		{
			Addons.Add(GetType().Assembly);
		}

		public bool DoesHookExist(string hookName)
		{
			using (TimeMeasure.New($"DoesHookExist: {hookName}"))
			{
				foreach (var addon in Addons)
				{
					foreach (var type in addon.GetTypes())
					{
						var hook = type.GetCustomAttribute<Hook>();
						if (hook == null) continue;

						if (hook.Name == hookName) return true;
					}
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
					Logger.Instance.Warn($" No plugin is using '{hookName}'. Unpatching.");
					UninstallHooks(hookName);
				}
			}
		}

		public void InstallHooks(string hookName, bool doRequires = true)
		{
			if (!DoesHookExist(hookName)) return;
			if (!IsPatched(hookName)) Carbon.Logger.Instance.Warn($"Found '{hookName}'...");

			new HookInstallerThread { HookName = hookName, DoRequires = doRequires, Processor = this }.Start();
		}
		public void UninstallHooks(string hookName)
		{
			using (TimeMeasure.New($"UninstallHooks: {hookName}"))
			{
				if (Patches.TryGetValue(hookName, out var instance))
				{
					foreach (var patch in instance.Patches)
					{
						if (string.IsNullOrEmpty(instance.Id)) continue;

						patch.UnpatchAll(instance.Id);
					}

					instance.Patches.Clear();
				}
			}
		}

		internal Type[] GetMatchedParameters(Type type, string methodName, ParameterInfo[] parameters)
		{
			var list = Pool.GetList<Type>();

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

						if (param.ParameterType.FullName.Replace("&", "") == otherParam.ParameterType.FullName.Replace("&", ""))
						{
							list.Add(param.ParameterType);
						}
					}
					catch { }
				}
			}

			var result = list.ToArray();
			Pool.FreeList(ref list);
			return result;
		}

		public class HookInstance
		{
			public string Id { get; set; }
			public int Hooks { get; set; } = 0;
			public List<HarmonyInstance> Patches { get; } = new List<HarmonyInstance>();
		}

		public class HookInstallerThread : ThreadedJob
		{
			public string HookName;
			public bool DoRequires = true;
			public CarbonAddonProcessor Processor;

			public override void ThreadFunction()
			{
				foreach (var addon in Processor.Addons)
				{
					foreach (var type in addon.GetTypes())
					{
						try
						{
							var parameters = type.GetCustomAttributes<Hook.Parameter>();
							var hook = type.GetCustomAttribute<Hook>();
							var args = string.Empty;

							if (parameters != null)
							{
								foreach (var parameter in parameters)
								{
									args += $"_[{parameter.Type.Name}]{parameter.Name}";
								}
							}

							if (hook == null) continue;

							if (hook.Name == HookName)
							{
								var patchId = $"{hook.Name}.{args}";
								var patch = type.GetCustomAttribute<Hook.Patch>();
								var hookInstance = (HookInstance)null;

								if (!Processor.Patches.TryGetValue(HookName, out hookInstance))
								{
									Processor.Patches.Add(HookName, hookInstance = new HookInstance());
								}

								if (hookInstance.Patches.Any(x => x != null && x.Id == patchId)) continue;

								if (DoRequires)
								{
									var requires = type.GetCustomAttributes<Hook.Require>();

									if (requires != null)
									{
										foreach (var require in requires)
										{
											if (require.Hook == HookName) continue;

											Processor.InstallHooks(require.Hook, false);
										}
									}
								}

								var originalParameters = Pool.GetList<Type>();
								var prefix = type.GetMethod("Prefix");
								var postfix = type.GetMethod("Postfix");
								var transplier = type.GetMethod("Transplier");

								foreach (var param in (prefix ?? postfix ?? transplier).GetParameters())
								{
									originalParameters.Add(param.ParameterType);
								}
								var originalParametersResult = originalParameters.ToArray();

								var matchedParameters = patch.UseProvidedParameters ? originalParametersResult : Processor.GetMatchedParameters(patch.Type, patch.Method, (prefix ?? postfix ?? transplier).GetParameters());
								var instance = HarmonyInstance.Create(patchId);
								var originalMethod = patch.Type.GetMethod(patch.Method, matchedParameters);

								instance.Patch(originalMethod,
									prefix: prefix == null ? null : new HarmonyMethod(prefix),
									postfix: postfix == null ? null : new HarmonyMethod(postfix),
									transpiler: transplier == null ? null : new HarmonyMethod(transplier));
								hookInstance.Patches.Add(instance);
								hookInstance.Id = patchId;

								Logger.Instance.Warn($" Patched {HookName}{args}...");

								Pool.Free(ref matchedParameters);
								Pool.Free(ref originalParametersResult);
								Pool.FreeList(ref originalParameters);
							}
						}
						catch (Exception exception)
						{
							Logger.Instance.Error($"Couldn't patch hook '{HookName}' ({type.FullName})", exception);
						}
					}

				}
			}
		}
	}
}
