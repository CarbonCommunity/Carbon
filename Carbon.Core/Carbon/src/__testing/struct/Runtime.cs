
using System;
using System.Collections.Generic;
using System.Reflection;

///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

namespace Carbon.Hooks;

internal struct Runtime
{
	public Exception lastError;
	public List<string> subscribers;
	public CarbonHookEx.State status;

	// Harmony
	public HarmonyLib.Harmony harmony;
	public MethodInfo prefix, postfix, transpiler;
}
