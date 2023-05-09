using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Hooks;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;
#pragma warning disable IDE0051

internal sealed class HookManager : AddonManager
{
	private readonly string[] _directories =
	{
		Context.CarbonHooks,
	};

	[MethodImpl(MethodImplOptions.NoInlining)]
	public override Assembly Load(string file, string requester = null)
	{
		if (requester is null)
		{
			MethodBase caller = new StackFrame(1).GetMethod();
			requester = $"{caller.DeclaringType}.{caller.Name}";
		}

		IReadOnlyList<string> blacklist = null;
		IReadOnlyList<string> whitelist = null;

		try
		{
			// Packed files will not work with the sandbox as they will fail
			// to be read when doing the validation process.

			switch (Path.GetExtension(file))
			{
				case ".dll":
					IEnumerable<Type> types;
					// before changing this line, look at the warning above..
					Assembly asm = _loader.Load(file, requester, _directories, blacklist, whitelist)?.Assembly
						?? throw new ReflectionTypeLoadException(null, null, null);
					// -----------------------------------------------------------------------------

					if (AssemblyManager.IsType<Patch>(asm, out types))
					{
						Logger.Debug($"Loading hooks file '{file}'");
						// TODO: Integrate part of HookManager here
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
			Logger.Error($"Error while loading hooks from '{file}'.");
			Logger.Error($"Either the file is corrupt or has an unsupported version.");
			return null;
		}
#if DEBUG
		catch (System.Exception e)
		{
			Logger.Error($"Failed loading hook '{file}'", e);

			return null;
		}
#else
		catch (System.Exception)
		{
			Logger.Error($"Failed loading hook '{file}'");

			return null;
		}
#endif
	}
}
