using System.Reflection;
using API.Hooks;
using HarmonyLib;

/*
 *
 * Copyright (c) 2022-2024 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public struct HookRuntime
{
	public string LastError;
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

public struct TaskStatus
{
	public int Static, Patch, Dynamic;
	public int Total { get => (Static + Patch + Dynamic); }
}
