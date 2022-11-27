using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

internal class CarbonHookEx : IDisposable
{
	private Runtime runtime;
	private readonly Metadata metadata;
	private readonly TypeInfo hookPatchMethod;


	internal string HookName
	{ get => metadata.HookName; }

	internal string Identifier
	{ get => metadata.Identifier; }

	internal bool IsStaticHook
	{ get => metadata.AlwaysApplyPatch; }

	internal bool IsHidden
	{ get => metadata.HideFromListings; }

	internal bool HasDependencies
	{ get => (metadata.DependsOn != null && metadata.DependsOn.Length > 0); }

	internal string[] Dependencies
	{ get => metadata.DependsOn; }

	internal string PatchMethodName
	{ get => hookPatchMethod.Name; }

	internal State Status
	{ get => runtime.status; set => runtime.status = value; }

	internal Exception LastError
	{ get => runtime.lastError; set => runtime.lastError = value; }

	internal bool IsLoaded
	{ get => runtime.status != State.Failure; }

	internal bool IsInstalled
	{ get => runtime.status == State.Success || runtime.status == State.Warning; }

	internal bool HasSubscribers
	{ get => runtime.subscribers?.Count > 0; }

	internal int SubscribersCount
	{ get => runtime.subscribers?.Count ?? 0; }


	internal bool IsSubscribedBy(string requester)
		=> runtime.subscribers?.Any(x => x == requester) ?? false;

	internal void AddSubscriber(string requester)
		=> runtime.subscribers.Add(requester);

	internal void RemoveSubscriber(string requester)
		=> runtime.subscribers.Remove(requester);


	public enum State
	{
		Inactive, Warning, Failure, Success
	}

	internal CarbonHookEx(TypeInfo type)
	{
		try
		{
			FieldInfo field = HarmonyLib.AccessTools.Field(type, "metadata") ?? null;
			metadata = field?.GetValue(null) as Metadata ?? null;

			if (type == null || field == null || metadata == null)
				throw new Exception($"Metadata type or field not found");

			hookPatchMethod = type;
			runtime.subscribers = new List<string>();
			runtime.harmony = new HarmonyLib.Harmony(metadata.Identifier);

			// cache the additional metadata about the hook
			runtime.prefix = HarmonyLib.AccessTools.Method(type, "Prefix") ?? null;
			runtime.postfix = HarmonyLib.AccessTools.Method(type, "Postfix") ?? null;
			runtime.transpiler = HarmonyLib.AccessTools.Method(type, "Transpiler") ?? null;

			if (runtime.prefix is null && runtime.postfix is null && runtime.transpiler is null)
				throw new Exception($"Patch method not found");
		}
		catch (System.Exception e)
		{
			Carbon.Logger.Error($"Error while parsing '{type.Name}'", e);
			return;
		}
	}

	internal bool ApplyPatch()
	{
		try
		{
			if (IsInstalled) return true;

			MethodInfo original = HarmonyLib.AccessTools.Method(
				metadata.TargetType, metadata.TargetMethod, metadata.TargetMethodArgs) ?? null;

			if (original is null)
				throw new Exception($"Target method not found");

			bool hasValidChecksum = IsChecksumValid(original);

			MethodInfo current = runtime.harmony.Patch(original,
				prefix: runtime.prefix == null ? null : new HarmonyLib.HarmonyMethod(runtime.prefix),
				postfix: runtime.postfix == null ? null : new HarmonyLib.HarmonyMethod(runtime.postfix),
				transpiler: runtime.transpiler == null ? null : new HarmonyLib.HarmonyMethod(runtime.transpiler)
			) ?? null;

			if (current is null)
				throw new Exception($"Harmony failed to execute");

			if (hasValidChecksum)
			{
				runtime.status = State.Success;
			}
			else
			{
				Logger.Warn($"Checksum validation failed for '{metadata.TargetType.Name}.{metadata.TargetMethod}''");
				runtime.status = State.Warning;
			}

			Logger.Debug($"Hook '{HookName}[{metadata.Identifier}]' patched '{metadata.TargetType.Name}.{metadata.TargetMethod}'", 2);
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while patching hook '{HookName}'", e);
			runtime.status = State.Failure;
			runtime.lastError = e;
			return false;
		}

		return true;

		/*
#if DEBUG
				catch (HarmonyException e)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine($" Couldn't patch hook '{HookName}' ({e.GetType()}: {type.FullName})");
					sb.AppendLine($">> hook:{HookName} index:{e.GetErrorIndex()} offset:{e.GetErrorOffset()}");
					sb.AppendLine($">> IL instructions:");

					foreach (var q in e.GetInstructionsWithOffsets())
						sb.AppendLine($"\t{q.Key.ToString("X4")}: {q.Value}");

					Logger.Error(sb.ToString(), e);
					sb = default;
				}
#endif
*/
	}

	internal bool RemovePatch()
	{
		try
		{
			if (!IsInstalled) return true;
			runtime.harmony.UnpatchAll(Identifier);

			Logger.Debug($"Hook '{HookName}[{metadata.Identifier}]' unpatched '{metadata.TargetType.Name}.{metadata.TargetMethod}'", 2);
			runtime.status = State.Inactive;
			return true;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while unpatching hook '{HookName}'", e);
			runtime.status = State.Failure;
			runtime.lastError = e;
			return false;
		}
	}

	private bool IsChecksumValid(MethodInfo original)
	{
		return true;
	}

	public void Dispose()
	{
		RemovePatch();
		runtime.harmony = null;
	}
}