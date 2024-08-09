using System.Reflection;
using API.Hooks;
using HarmonyLib;

namespace Carbon.Hooks;

public struct HookRuntime
{
	public string LastError;
	public HookState Status;

	public Harmony HarmonyHandler;
	public MethodInfo Prefix;
	public MethodInfo Postfix;
	public MethodInfo Transpiler;
}

public struct Subscription
{
	public string Identifier, Subscriber;

	public Subscription(string id, string sub)
	{
		Identifier = id;
		Subscriber = sub;
	}
}

public struct TaskStatus
{
	public int Static;
	public int Patch;
	public int Dynamic;
	public int Metadata;

	public readonly int Total => Static + Patch + Dynamic + Metadata;
}
