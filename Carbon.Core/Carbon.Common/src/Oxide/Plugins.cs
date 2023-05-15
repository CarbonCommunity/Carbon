using System.Linq;
using Carbon;
using Carbon.Core;
using Facepunch;
using Oxide.Core.Plugins;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries;

public class Plugins : Library
{
	public PluginManager PluginManager { get; private set; } = new();

	public bool IsGlobal => true;

	public Plugins(PluginManager pluginmanager)
	{
		PluginManager = pluginmanager ?? new();
	}

	public bool Exists(string name)
	{
		return Community.Runtime.Plugins.Plugins.Any(x => x.Name == name);
	}

	public Plugin Find(string name)
	{
		name = name.Replace(" ", "");

		foreach (var mod in ModLoader.LoadedPackages)
		{
			foreach (var plugin in mod.Plugins)
			{
				if (plugin.Name.Replace(" ", "").Replace(".", "") == name) return plugin;
			}
		}

		return null;
	}

	public Plugin[] GetAll()
	{
		var list = Pool.GetList<Plugin>();
		foreach (var mod in ModLoader.LoadedPackages)
		{
			list.AddRange(mod.Plugins);
		}

		var result = list.ToArray();
		Pool.FreeList(ref list);
		return result;
	}

	public object CallHook(string hook)
	{
		return HookCaller.CallStaticHook(hook);
	}
	public object CallHook(string hook, object arg1)
	{
		return HookCaller.CallStaticHook(hook, arg1);
	}
	public object CallHook(string hook, object arg1, object arg2)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
	}
	public object CallHook(string hook, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
	{
		return HookCaller.CallStaticHook(hook, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
	}
	public object CallHook(string hook, object[] args)
	{
		return args?.Length switch
		{
			1 => HookCaller.CallStaticHook(hook, args[0]),
			2 => HookCaller.CallStaticHook(hook, args[0], args[1]),
			3 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2]),
			4 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3]),
			5 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4]),
			6 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5]),
			7 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
			8 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
			9 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
			10 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
			11 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
			12 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
			13 => HookCaller.CallStaticHook(hook, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
			_ => HookCaller.CallStaticHook(hook),
		};
	}
}
