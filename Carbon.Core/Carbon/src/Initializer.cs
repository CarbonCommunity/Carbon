using System;
using System.Reflection;
using System.Threading;
using API.Assembly;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

public class Initializer : ICarbonComponent
{
	public void Awake(EventArgs args)
	{
		Logger.Debug($"A new instance of '{this}' created");
	}

	public void OnEnable(EventArgs args)
	{
		Logger.Debug($"Triggered '{this}' OnEnable");
	}

	public void OnDisable(EventArgs args)
	{
		Logger.Debug($"Triggered '{this}' OnDisable");
	}

	public void OnLoaded(EventArgs args)
	{
		try
		{
			if (Type.GetType("Oxide.Core.Interface, Oxide.Core") is not null)
			{
				Logger.Log(Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"  ________ _______ ______ _______ _______ _______ _______ " + Environment.NewLine +
					@" |  |  |  |   _   |   __ \    |  |_     _|    |  |     __|" + Environment.NewLine +
					@" |  |  |  |       |      <       |_|   |_|       |    |  |" + Environment.NewLine +
					@" |________|___|___|___|__|__|____|_______|__|____|_______|" + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"    WE HAVE DETECTED YOUR SERVER IS STILL USING OXIDE.    " + Environment.NewLine +
					@"    CARBON WILL NOT WORK PROPERLY.                        " + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"    REMOVE THE 'RustDedicated_Data\Managed' FOLDER AND    " + Environment.NewLine +
					@"    EXECUTE YOUR STEAMCMD UPDATE PROCESS AGAIN.           " + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"    THIS SERVER WILL BE TERMINATED IN 60 SECONDS.         " + Environment.NewLine +
					@"    THANK YOU <3                                          " + Environment.NewLine +
					@"                                                          " + Environment.NewLine
				);

				Thread.Sleep(60000);
				UnityEngine.Application.Quit();
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
					@"    THE SERVER ASSEMBLY CODE IS NOT PUBLICIZED.           " + Environment.NewLine +
					@"    CARBON WILL NOT WORK PROPERLY.                        " + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"    PLEASE MAKE SURE UNITY DOORSTOP IS BEING EXECUTED.    " + Environment.NewLine +
					@"    IF THE PROBLEM PRESIST PLEASE OPEN A NEW ISSUE AT     " + Environment.NewLine +
					@"    GITHUB OR ASK FOR SUPPORT ON OUR DISCORD.             " + Environment.NewLine +
					@"                                                          " + Environment.NewLine +
					@"    THIS SERVER WILL BE TERMINATED IN 60 SECONDS.         " + Environment.NewLine +
					@"    THANK YOU <3                                          " + Environment.NewLine +
					@"                                                          " + Environment.NewLine
				);

				Thread.Sleep(60000);
				UnityEngine.Application.Quit();
				return;
			}
		}
		catch (Exception e)
		{
			Logger.Error("Unable to assert assembly status.", e);
			return;
		}
		Logger.Log(Environment.NewLine +
			@"                                                                      " + Environment.NewLine +
			@"  ______ _______ ______ ______ _______ _______ _______ ______ _______ " + Environment.NewLine +
			@" |      |   _   |   __ \   __ \       |    |  |   _   |   __ \   _   |" + Environment.NewLine +
			@" |   ---|       |      <   __ <   -   |       |       |      <       |" + Environment.NewLine +
			@" |______|___|___|___|__|______/_______|__|____|___|___|___|__|___|___|" + Environment.NewLine +
			@"                                                discord.gg/eXPcNKK4yd " + Environment.NewLine +
			@"                                                                      " + Environment.NewLine
		);

		try
		{
			Logger.Log("Initializing...");

			if (CommunityInternal.InternalRuntime == null) CommunityInternal.InternalRuntime = new CommunityInternal();
			else CommunityInternal.InternalRuntime?.Uninitalize();

			CommunityInternal.InternalRuntime.Initialize();
		}
		catch (System.Exception e)
		{
			Logger.Error("Unable to initialize.", e.InnerException ?? e);
			return;
		}
	}

	public void OnUnloaded(EventArgs args)
	{	
		Logger.Log("Uninitalizing...");
		CommunityInternal.InternalRuntime?.Uninitalize();
		CommunityInternal.InternalRuntime = null;
	}
}
