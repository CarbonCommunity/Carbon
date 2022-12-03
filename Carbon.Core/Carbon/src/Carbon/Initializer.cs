///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Reflection;
using System.Threading;

namespace Carbon.Core;

public class Initializer : IHarmonyModHooks
{
	public void OnLoaded(OnHarmonyModLoadedArgs args)
	{
		try
		{
			Type t = Type.GetType("ServerMgr, Assembly-CSharp");
			MethodInfo m = t.GetMethod("Shutdown", (BindingFlags)62) ?? null;
			if (m == null || !m.IsPublic)
			{
				Carbon.Logger.Log(Environment.NewLine +
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
			else
			{
				Carbon.Logger.Log(Environment.NewLine +
					@"                                               " + Environment.NewLine +
					@"  ______ _______ ______ ______ _______ _______ " + Environment.NewLine +
					@" |      |   _   |   __ \   __ \       |    |  |" + Environment.NewLine +
					@" |   ---|       |      <   __ <   -   |       |" + Environment.NewLine +
					@" |______|___|___|___|__|______/_______|__|____|" + Environment.NewLine +
					@"                         discord.gg/eXPcNKK4yd " + Environment.NewLine +
					@"                                               " + Environment.NewLine
				);

				Carbon.Logger.Log("Initializing...");

				if (Community.Runtime == null) Community.Runtime = new Community();
				else Community.Runtime?.Uninitalize();

				Community.Runtime.Initialize();
			}
		}
		catch (Exception ex)
		{
			Carbon.Logger.Error("Unable to assert assembly status.", ex);
			return;
		}
	}

	public void OnUnloaded(OnHarmonyModUnloadedArgs args)
	{
		Carbon.Logger.Log("Uninitalizing...");
		Community.Runtime?.Uninitalize();
		Community.Runtime = null;
	}
}
