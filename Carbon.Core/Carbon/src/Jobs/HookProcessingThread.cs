///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbon.Base;
using Carbon.Core;
using Carbon.Hooks;
using Carbon.Processors;
using Facepunch;

namespace Carbon.Jobs
{
	public class HookProcessingThread : BaseThreadedJob
	{
		public string HookName;
		public bool DoRequires = true;
		public bool OnlyAlwaysPatchedHooks = false;
		public HookProcessor Processor;

		public override void ThreadFunction()
		{
			if (string.IsNullOrEmpty(HookName)) return;

			foreach (var type in Defines.Carbon.GetTypes())
			{
				try
				{
					var parameters = type.GetCustomAttributes<Hook.Parameter>();
					var hook = type.GetCustomAttribute<Hook>();
					var args = $"[{type.Name}]_";

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
						var patchId = $"{hook.Name}{args}";
						var patch = type.GetCustomAttribute<Hook.Patch>();
						var hookInstance = (HookProcessor.HookInstance)null;

						if (!Processor.Patches.TryGetValue(HookName, out hookInstance))
						{
							Processor.Patches.Add(HookName, hookInstance = new HookProcessor.HookInstance
							{
								AlwaysPatched = type.GetCustomAttribute<Hook.AlwaysPatched>() != null
							});
						}

						if (hookInstance.AlwaysPatched && !OnlyAlwaysPatchedHooks) continue;

						if (hookInstance.Patches.Any(x => x != null && x is HarmonyLib.Harmony harmony && harmony.Id == patchId)) continue;

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

						var originalParameters = new List<Type>();
						var prefix = type.GetMethod("Prefix");
						var postfix = type.GetMethod("Postfix");
						var transpiler = type.GetMethod("Transpiler");

						foreach (var param in (prefix ?? postfix ?? transpiler).GetParameters())
						{
							originalParameters.Add(param.ParameterType);
						}
						var originalParametersResult = originalParameters.ToArray();

						var matchedParameters = patch.UseProvidedParameters ? originalParametersResult : Processor.GetMatchedParameters(patch.Type, patch.Method, (prefix ?? postfix ?? transpiler).GetParameters());

						var instance = new HarmonyLib.Harmony(patchId);
						var originalMethod = patch.Type.GetMethod(patch.Method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, matchedParameters, default);

						instance.Patch(originalMethod,
							prefix: prefix == null ? null : new HarmonyLib.HarmonyMethod(prefix),
							postfix: postfix == null ? null : new HarmonyLib.HarmonyMethod(postfix),
							transpiler: transpiler == null ? null : new HarmonyLib.HarmonyMethod(transpiler));
						hookInstance.Patches.Add(instance);
						hookInstance.Id = patchId;

						Logger.Debug($" -> Patched '{hook.Name}' <- {patchId}", 2);

						Pool.Free(ref matchedParameters);
						Pool.Free(ref originalParametersResult);
						originalParameters.Clear();
						originalParameters = null;
					}
				}
				catch (Exception exception)
				{
					Logger.Error($" Couldn't patch hook '{HookName}' ({type.FullName})", exception);
				}
			}
		}
	}
}
