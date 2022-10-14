///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using Carbon;
using Carbon.Core;

namespace Oxide.Core
{
	public class Interface
	{
		public static OxideMod Oxide { get; set; } = new OxideMod();
		public static OxideMod uMod => Oxide;

		public static void Initialize()
		{
			Oxide.Load();
			Carbon.Logger.Log($"  Instance Directory: {Oxide.InstanceDirectory}");
			Carbon.Logger.Log($"  Root Directory: {Oxide.RootDirectory}");
			Carbon.Logger.Log($"  Config Directory: {Oxide.ConfigDirectory}");
			Carbon.Logger.Log($"  Data Directory: {Oxide.DataDirectory}");
			Carbon.Logger.Log($"  Lang Directory: {Oxide.LangDirectory}");
			Carbon.Logger.Log($"  Log Directory: {Oxide.LogDirectory}");
			Carbon.Logger.Log($"  Plugin Directory: {Oxide.PluginDirectory}");
		}

		public static OxideMod GetMod() => Oxide;

		public static object CallHook(string hookName)
		{
			return HookExecutor.CallStaticHook(hookName);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate);
		}
		public static object CallHook(string hookName, object arg1)
		{
			return HookExecutor.CallStaticHook(hookName, arg1);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1);
		}
		public static object CallHook(string hookName, object arg1, object arg2)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4, arg5);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4, arg5);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4, arg5, arg6);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
		public static object CallHook(string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookExecutor.CallStaticHook(hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
		{
			return HookExecutor.CallStaticDeprecatedHook(oldHook, newHook, expireDate, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
		public static object Call(string hook, params object[] args)
		{
			return HookExecutor.CallStaticHook(hook, args);
		}
	}
}
