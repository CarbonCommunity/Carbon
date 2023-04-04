using System;
using System.Collections.Generic;
using System.Reflection;
using ConVar;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public static class HarmonyLoader
{
	public class HarmonyMod
	{
		public string Name { get; set; }
		public string HarmonyId { get; set; }
		public HarmonyLib.Harmony Harmony { get; set; }
		public Assembly Assembly { get; set; }
		public Type[] AllTypes { get; set; }
		public List<IHarmonyModHooks> Hooks { get; } = new();
	}

	public static List<HarmonyMod> loadedMods = new();
}
