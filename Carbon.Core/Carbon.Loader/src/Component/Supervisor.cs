/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

using System;
using Carbon.LoaderEx.Common;

namespace Carbon.LoaderEx.Supervisor;

public static class Core
{
	public static bool IsStarted
	{ get => HarmonyLoaderEx.GetInstance().IsLoaded("Carbon.dll"); }

	public static void Start()
		=> HarmonyLoaderEx.GetInstance().Load("Carbon.dll");

	public static void Exit()
		=> HarmonyLoaderEx.GetInstance().Unload("Carbon.dll");

	public static void Update(object os, object release, Action<bool> callback = null)
		=> Updater.UpdateCarbon(os, release, callback);

	public static void Reboot()
	{
		Exit();
		Start();
	}
}
