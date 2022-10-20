///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System.Collections.Generic;

namespace Carbon.Common;

internal class HarmonyPlugin : CarbonReference
{
	public string identifier;

	public List<IHarmonyModHooks> hooks
		= new List<IHarmonyModHooks>();

	public HarmonyLib.Harmony harmonyInstance;
}