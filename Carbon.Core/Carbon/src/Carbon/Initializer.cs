///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Reflection;
using Carbon.Hooks;
using UnityEngine;

namespace Carbon.Core
{
	public class Initializer : IHarmonyModHooks
	{
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			var oldMod = PlayerPrefs.GetString(Harmony_Load.CARBON_LOADED);

			if (!Assembly.GetExecutingAssembly().FullName.StartsWith(oldMod))
			{
				Community.Runtime?.Uninitalize();
				HarmonyLoader.TryUnloadMod(oldMod);
				Carbon.Logger.Warn($"Unloaded previous: {oldMod}");
				Community.Runtime = null;
			}

#if UNIX
			try
			{
				Type t = Type.GetType("ServerMgr, Assembly-CSharp");
				MethodInfo m = t.GetMethod("Shutdown", BindingFlags.Public | BindingFlags.Instance) ?? null;
				if (m == null || !m.IsPublic) return;
			}
			catch (Exception ex)
			{
				Carbon.Logger.Error("Unable to assert assembly status.", ex);
				return;
			}
#endif

			Carbon.Logger.Log(
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

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}

	[Harmony.HarmonyPatch(typeof(Bootstrap), "StartupShared")]
	public class Init
	{
		public static void Prefix() { }
	}
}
