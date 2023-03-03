
using System;
using System.Reflection;
using API.Hooks;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public struct HookRuntime
{
	public Exception LastError;
	public HookState Status;

	// Harmony
	public Harmony HarmonyHandler;
	public MethodInfo Prefix, Postfix, Transpiler;
}

public struct Subscription
{
	public string Identifier, Subscriber;

	public Subscription(string id, string sub)
	{ Identifier = id; Subscriber = sub; }
}
