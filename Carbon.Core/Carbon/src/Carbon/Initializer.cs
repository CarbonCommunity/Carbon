using System;
using System.IO;
using System.Reflection;
using System.Threading;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

public class Initializer : IHarmonyModHooks
{
	public void OnLoaded(OnHarmonyModLoadedArgs args)
	{
		try
		{
			if (IsOxideAround())
			{
				Logger.Log(Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"  ________ _______ ______ _______ _______ _______ _______ " + Environment.NewLine +
					@" |  |  |  |   _   |   __ \    |  |_     _|    |  |     __|" + Environment.NewLine +
					@" |  |  |  |       |      <       |_|   |_|       |    |  |" + Environment.NewLine +
					@" |________|___|___|___|__|__|____|_______|__|____|_______|" + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"   OXIDE  IS  CURRENTLY  LOADED  IN  YOUR  GAME  FOLDER   " + Environment.NewLine +
					@"   DIRECTORY: RUSTDEDICATED_DATA/MANAGED. PLEASE DELETE   " + Environment.NewLine +
					@"   ALL OXIDE-RELATED DLLS  IN  ORDER FOR CARBON TO WORK   " + Environment.NewLine +
					@"   PROPERLY.                       THANK YOU VERY MUCH!   " + Environment.NewLine +
					@"                                                          " + Environment.NewLine
				);

				Thread.Sleep(15000);
				return;
			}
		}
		catch (Exception e)
		{
			Logger.Error("Unable to assert assembly status.", e);
			return;
		}

		try
		{
			Type t = Type.GetType("ServerMgr, Assembly-CSharp");
			MethodInfo method = t.GetMethod("Shutdown", (BindingFlags)62) ?? null;

			if (method == null || !method.IsPublic)
			{
				Logger.Log(Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"  ________ _______ ______ _______ _______ _______ _______ " + Environment.NewLine +
					@" |  |  |  |   _   |   __ \    |  |_     _|    |  |     __|" + Environment.NewLine +
					@" |  |  |  |       |      <       |_|   |_|       |    |  |" + Environment.NewLine +
					@" |________|___|___|___|__|__|____|_______|__|____|_______|" + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"   THE ASSEMBLER CODE IS NOT PUBLICIZED, CARBON WILL NOT  " + Environment.NewLine +
					@"   WORK AS EXPECTED.  PLEASE MAKE SURE UNITY DOORSTOP IS  " + Environment.NewLine +
					@"   BEING EXECUTED.  IF THE PROBLEM PRESIST PLEASE OPEN A  " + Environment.NewLine +
					@"   NEW ISSUE AT GITHUB OR ASK FOR SUPPORT ON OUR DISCORD  " + Environment.NewLine +
					@"                                                          " + Environment.NewLine
				);

				Thread.Sleep(15000);
				return;
			}
		}
		catch (Exception e)
		{
			Logger.Error("Unable to assert assembly status.", e);
			return;
		}

		Logger.Log(Environment.NewLine +
			@"                                               " + Environment.NewLine +
			@"  ______ _______ ______ ______ _______ _______ " + Environment.NewLine +
			@" |      |   _   |   __ \   __ \       |    |  |" + Environment.NewLine +
			@" |   ---|       |      <   __ <   -   |       |" + Environment.NewLine +
			@" |______|___|___|___|__|______/_______|__|____|" + Environment.NewLine +
			@"                         discord.gg/eXPcNKK4yd " + Environment.NewLine +
			@"                                               " + Environment.NewLine
		);

		try
		{
			Logger.Log("Initializing...");

			if (Community.Runtime == null) Community.Runtime = new Community();
			else Community.Runtime?.Uninitalize();

			Community.Runtime.Initialize();
		}
		catch (System.Exception e)
		{
			Logger.Error("Unable to initialize.", e.InnerException ?? e);
			return;
		}
	}

	public void OnUnloaded(OnHarmonyModUnloadedArgs args)
	{
		Logger.Log("Uninitalizing...");
		Community.Runtime?.Uninitalize();
		Community.Runtime = null;
	}

	internal static bool IsOxideAround()
	{
		return File.Exists(Path.Combine(Defines.GetRustManagedFolder(), "Oxide.Core.dll"));
	}
}
