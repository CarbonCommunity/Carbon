﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using API.Abstracts;
using API.Commands;
using API.Events;
using API.Hooks;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Pooling;
using Network;

/*
 *
 * Copyright (c) 2022-2024 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public sealed class PatchManager : CarbonBehaviour, IPatchManager, IDisposable
{
	internal List<HookEx> _patches { get; set; }
	internal List<HookEx> _staticHooks { get; set; }
	internal List<HookEx> _dynamicHooks { get; set; }
	internal List<HookEx> _metadataHooks { get; set; }

	// TODO --------------------------------------------------------------------
	// Allows patch uninstallation on the correct order and should be replaced
	// with Harmony After/Before mechanics. To be able to do that we need to
	// modify the autogen tool to take into consideration the offset created by
	// the first applied patch when dealing with the InjectionIndex of the
	// second patch.
	internal List<HookEx> _installed { get; set; }

#if DEBUG
	// Number of patches applied by update cycle
	private readonly int PatchLimitPerCycle = 50;
#else
	// Number of patches applied by update cycle
	private readonly int PatchLimitPerCycle = 25;
#endif

	private Stopwatch sw;
	private bool _doReload;
	private Queue<string> _workQueue;
	private List<Subscription> _subscribers;
	private readonly Dictionary<string, string> _checksums = new();
	private static bool InitialHooksInstalled = true;
	private static bool ForceUpdate;
	private static bool InitialOnEnable;

	public void Enqueue(string identifier)
	{
		if (_workQueue.Contains(identifier))
		{
			return;
		}

		_workQueue.Enqueue(identifier);
	}

	private static readonly string[] Files =
	{
		"Carbon.Hooks.Base.dll",
		"Carbon.Hooks.Community.dll",
		"Carbon.Hooks.Oxide.dll",
	};

	private static readonly string[] WarnExclusions =
	{
		"IOnServerCommand",
		"IOnRunCommandLine",
		"SingleCharCmdPrefix [patch]"
	};

	private void Awake()
	{
		Logger.Log($"Initializing {this}..");
		sw = new Stopwatch();

		_dynamicHooks = new List<HookEx>();
		_installed = new List<HookEx>();
		_patches = new List<HookEx>();
		_staticHooks = new List<HookEx>();
		_metadataHooks = new List<HookEx>();
		_subscribers = new List<Subscription>();
		_workQueue = new Queue<string>();

		Community.Runtime.CommandManager.RegisterCommand(new Command.RCon
		{
			Name = "c.hooks",
			Callback = (arg) => CMDHookInfo(arg)
		}, out string _);

		enabled = false;

		var doHookUpdate = !CommandLineEx.GetArgumentExists("+carbon.skiphookupdates");

		if (doHookUpdate)
		{
			Logger.Log("Updating hooks...");

			Updater.DoUpdate((bool result) =>
			{
				if (!result)
					Logger.Error($"Unable to update the hooks at this time, please try again later");
				enabled = true;
			});
		}
		else
		{
			Logger.Log("Hook updates disabled, loading from disk...");
			Invoke(() => enabled = true, 0.1f);
		}
	}

	private void OnEnable()
	{
		_patches.Clear();
		_staticHooks.Clear();
		_dynamicHooks.Clear();

		foreach (string file in Files)
		{
			//FIXMENOW
			//string path = Path.Combine(Defines.GetManagedFolder(), file);

			//if (Supervisor.ASM.IsLoaded(Path.GetFileName(path)))
			//	Supervisor.ASM.UnloadModule(path, false);

			LoadHooksFromFile(file);
		}

		if (_patches.Count > 0)
		{
			Logger.Debug($" - Installing patches");
			// I don't like this, patching stuff that may not be used but for the
			// sake of time I will let it go for now but this needs to be reviewed.
			foreach (HookEx hook in _patches.Where(x => !x.IsInstalled && !x.HasDependencies()))
				Subscribe(hook.Identifier, "Carbon.Patch");
		}

		if (_staticHooks.Count > 0)
		{
			Logger.Debug($" - Installing static hooks");
			foreach (HookEx hook in _staticHooks.Where(x => !x.IsInstalled))
				Subscribe(hook.Identifier, "Carbon.Static");
		}

		if (!InitialOnEnable)
		{
			InitialOnEnable = true;

			if (ConVar.Global.skipAssetWarmup_crashes)
			{
				Community.Runtime.Events.Trigger(CarbonEvent.HooksInstalled, EventArgs.Empty);
			}
			else
			{
				Invoke(() => Community.Runtime.Events.Trigger(CarbonEvent.HooksInstalled, EventArgs.Empty), 1f);
			}
		}
	}

	private void OnDisable()
	{
		Logger.Log("Stopping hook processor...");

		foreach (HookEx item in _installed.AsEnumerable().Reverse())
		{
			if (!item.RemovePatch())
			{
				Logger.Warn($" Failed uninstalling patch: {item.HookFullName}[{item.Checksum}]");
				continue;
			}

			_installed.Remove(item);
		}

		if (!_doReload) return;

		Logger.Log("Reloading hook processor...");
		_doReload = false;
		enabled = true;
	}

	public void Fetch()
	{
		Community.Runtime.Events.Trigger(CarbonEvent.HookFetchStart, EventArgs.Empty);

		ShowAllPlayersLoading();

		foreach (var package in ModLoader.LoadedPackages)
		{
			foreach (var plugin in package.Plugins)
			{
				foreach (var hook in plugin.Hooks)
				{
					Unsubscribe(HookStringPool.GetOrAdd(hook), plugin.FileName);
				}
			}
		}

		try
		{
			OnDisable();
		}
		catch (Exception ex)
		{
			Logger.Error($"Reinstall failed: OnDisable failed", ex);
			return;
		}

		Logger.Warn(" Re-downloading hooks...");

		Updater.DoUpdate((bool result) =>
		{
			if (!result)
			{
				Logger.Error($"Unable to update the hooks at this time, please try again later");
				return;
			}

			OnEnable();

			foreach (var package in ModLoader.LoadedPackages)
			{
				foreach (var plugin in package.Plugins)
				{
					foreach (var hook in plugin.Hooks)
					{
						var name = HookStringPool.GetOrAdd(hook);

						if (plugin.IsHookIgnored(hook))
						{
							continue;
						}

						Subscribe(name, plugin.FileName);
					}
				}
			}

			Community.Runtime.Events.Trigger(CarbonEvent.HookFetchEnd, EventArgs.Empty);

			EndAllPlayersLoading();
		});
	}

	private void ShowAllPlayersLoading()
	{
		foreach (var player in BasePlayer.activePlayerList)
		{
			player.ClientRPCPlayer(null, player, "StartLoading");

			DisplayMessage(player.Connection, "Carbon Update", "Updating hooks...");
		}
	}

	private void EndAllPlayersLoading()
	{
		foreach (var player in BasePlayer.activePlayerList)
		{
			player.SendFullSnapshot();
			player.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
			player.SendNetworkUpdate();
		}
	}

	private void DisplayMessage(Connection con, string top, string bottom)
	{
		var writer = Net.sv.StartWrite();
		writer.PacketID(Message.Type.Message);
		writer.String(top);
		writer.String(bottom);
		writer.Send(new SendInfo(con));
	}

	private void OnDestroy()
	{
		Logger.Log("Destroying hook processor...");
		Dispose();
	}

	private void Update()
	{
		if (_workQueue.Count == 0)
		{
			return;
		}

		var limit = !InitialHooksInstalled || ForceUpdate ? int.MaxValue : PatchLimitPerCycle;

		while (_workQueue.Count > 0 && limit-- > 0)
		{
			string identifier = _workQueue.Dequeue();

			try
			{
				HookEx hook = GetHookById(identifier);

				bool hasSubscribers = HookHasSubscribers(hook.Identifier);
				bool isInstalled = hook.IsInstalled;
				bool hasValidChecksum = true;
				string checksum = null;

				if (!isInstalled)
				{
					if (hook.Status == HookState.Failure)
					{
						Logger.Warn($"A hook request for '{hook}' received:");
						Logger.Warn($" - The current status is FAILURE: {hook.LastError}");
						Logger.Warn($" - Check for possible errors on the log file");
					}

					checksum = GetMethodMSILHash(hook.GetTargetMethodInfo());

					hasValidChecksum =
						hook.IsChecksumIgnored || string.IsNullOrEmpty(hook.Checksum)
						|| string.IsNullOrEmpty(checksum) || checksum == hook.Checksum;
				}

				switch (hasSubscribers)
				{
					// Not installed but has subs, install
					case true when !isInstalled:
						if (!hook.ApplyPatch())
							throw new ApplicationException($"A general error occured while installing '{hook}'");
						Logger.Debug($"Installed hook '{hook}'", 1);
						_installed.Add(hook);
						break;

					// Installed but no subs found, uninstall
					case false when isInstalled:
						if (!hook.RemovePatch())
							throw new ApplicationException($"A general error occured while uninstalling '{hook}'");
						Logger.Debug($"Uninstalled hook '{hook}'", 1);
						_installed.Remove(hook);
						break;
				}

#if !(STAGING || RELEASE || AUX01 || AUX02)
				if (!hasValidChecksum)
				{
					if (!WarnExclusions.Contains(hook.HookFullName))
					{
						Logger.Warn($"Checksum validation failed for '{hook.TargetType}.{hook.TargetMethod}' [{hook.HookFullName}]");
						Logger.Debug($"live:{checksum} | expected:{hook.Checksum}");
						hook.SetStatus(HookState.Warning, "Invalid checksum");
					}
				}
#endif
			}
			catch (System.ApplicationException e)
			{
				Logger.Error(e.Message);
			}
			catch (System.Exception e)
			{
				Logger.Error($"HookManager.Update() failed at '{identifier}'", e);
			}
		}

		if (!InitialHooksInstalled)
		{
			InitialHooksInstalled = true;
		}

		if (ForceUpdate)
		{
			ForceUpdate = false;
		}
	}

	private void LoadHooksFromFile(string fileName)
	{
		try
		{
			// delegates asm loading to Carbon.Loader
			Assembly hooks = Community.Runtime.AssemblyEx.Hooks.Load(fileName, "HookManager.LoadHooksFromFile");

			if (hooks == null)
			{
				Logger.Error($"Error while loading hooks from '{fileName}'.");
				Logger.Error($"Either the file is corrupt or has an unsupported format/version.");
				return;
			}

			Type @base = hooks.GetType("API.Hooks.Patch")
				?? typeof(API.Hooks.Patch);

			Type attr = hooks.GetType("API.Hooks.HookAttribute.Patch")
				?? typeof(HookAttribute.Patch);

			IEnumerable<TypeInfo> types = hooks.DefinedTypes
				.Where(type => @base.IsAssignableFrom(type) && Attribute.IsDefined(type, attr));

			TaskStatus stats = LoadHooks(types);
			if (stats.Total == 0) return;

			Logger.Log($"- Loaded {stats.Total} hooks ({stats.Patch}/{stats.Static}/{stats.Dynamic})"
				+ $" from file '{Path.GetFileName(fileName)}' in {sw.ElapsedMilliseconds}ms");
		}
		catch (System.Exception)
		{
			Logger.Error($"- Error while loading hooks from file '{Path.GetFileName(fileName)}'");
			return;
		}
	}

	public void LoadHooksFromType(Type type)
	{
		Type @base = typeof(API.Hooks.Patch);
		Type attr = typeof(HookAttribute.Patch);

		IEnumerable<TypeInfo> types = type.GetNestedTypes()
			.Where(type => @base.IsAssignableFrom(type) && Attribute.IsDefined(type, attr))
			.Select(x => x.GetTypeInfo());

		TaskStatus stats = LoadHooks(types);
		if (stats.Total == 0) return;

		Logger.Log($"- Loaded {stats.Total} hooks ({stats.Patch}/{stats.Static}/{stats.Dynamic})"
			+ $" from type '{type}' in {sw.ElapsedMilliseconds}ms");
	}

	private TaskStatus LoadHooks(IEnumerable<TypeInfo> types)
	{
		TaskStatus retvar = new();
		sw.Restart();

		foreach (TypeInfo type in types)
		{
			try
			{
				HookEx hook = new HookEx(type)
					?? throw new Exception($"Hook is null, this is a bug");

				if (hook.Options.HasFlag(HookFlags.MetadataOnly))
				{
					hook.SetStatus(HookState.Inactive);
					retvar.Metadata++;
					_metadataHooks.Add(hook);
					HookStringPool.GetOrAdd(hook.HookName);
					continue;
				}

				if (IsHookLoaded(hook))
				{
					var assembly = type.Assembly.GetName();
					Logger.Warn($" Attempted to install duplicate hook '{hook}' (from {assembly.Name})");
					continue;
				}

				if (hook.IsPatch)
				{
					retvar.Patch++;
					_patches.Add(hook);
					Logger.Debug($"Loaded patch '{hook}'", 4);
					if (!hook.HasDependencies()) Subscribe(hook.Identifier, "Carbon.Patch");
				}
				else if (hook.IsStaticHook)
				{
					retvar.Static++;
					_staticHooks.Add(hook);
					Logger.Debug($"Loaded static hook '{hook}'", 4);
					Subscribe(hook.Identifier, "Carbon.Static");
				}
				else
				{
					retvar.Dynamic++;
					_dynamicHooks.Add(hook);
					Logger.Debug($"Loaded dynamic hook '{hook}'", 4);
				}

				HookStringPool.GetOrAdd(hook.HookName);
			}
			catch (System.Exception e)
			{
				Logger.Error($"Error while parsing '{type.Name}'", e);
			}
		}

		sw.Stop();
		return retvar;
	}

	private IEnumerable<HookEx> GetHookDependencyTree(HookEx hook)
	{
		return hook.Dependencies.SelectMany(x => GetHookByFullName(x).SelectMany(y => GetHookDependencyTree(y).Concat(new [] { y })));
	}

	private IEnumerable<HookEx> GetHookDependantTree(HookEx hook)
	{
		return _patches.Where(x => x.Dependencies.Contains(hook.HookFullName)).SelectMany(x => GetHookDependantTree(x).Concat(new [] { x }));
	}

	private IEnumerable<HookEx> LoadedHooks
	{ get => _patches.Concat(_dynamicHooks).Concat(_staticHooks).Where(x => x.IsLoaded); }

	private IEnumerable<HookEx> Hooks
	{ get => _patches.Concat(_dynamicHooks).Concat(_staticHooks).Concat(_metadataHooks); }

	private IEnumerable<HookEx> GetHookByName(string name)
		=> LoadedHooks.Where(x => x.HookName == name) ?? null;

	private IEnumerable<HookEx> GetHookByFullName(string name)
		=> LoadedHooks.Where(x => x.HookFullName == name) ?? null;

	private HookEx GetHookById(string identifier)
		=> LoadedHooks.FirstOrDefault(x => x.Identifier == identifier) ?? null;

	private IEnumerable<HookEx> GetHookByNameAll(string name)
		=> Hooks.Where(x => x.HookName == name) ?? null;

	private IEnumerable<HookEx> GetHookByFullNameAll(string name)
		=> Hooks.Where(x => x.HookFullName == name) ?? null;

	private HookEx GetHookByIdAll(string identifier)
		=> Hooks.FirstOrDefault(x => x.Identifier == identifier) ?? null;

	internal bool IsHookLoaded(HookEx hook)
		=> LoadedHooks.Any(x => x.HookFullName == hook.HookFullName && x.TargetType == hook.TargetType && x.TargetMethod == hook.TargetMethod && (x.TargetMethodArgs?.SequenceEqual(hook.TargetMethodArgs) ?? true));

	public bool IsHook(string hookName)
	{
		var hooks = GetHookByNameAll(hookName);
		return hooks.Any();
	}

	public bool IsHookLoaded(string hookName)
	{
		var hooks = GetHookByName(hookName);
		return hooks.Any() && hooks.Any(IsHookLoaded);
	}

	public void ForceUpdateHooks()
	{
		ForceUpdate = true;
		Update();
	}

	public IEnumerable<IHook> LoadedPatches
	{ get => _patches; }

	public IEnumerable<IHook> InstalledPatches
	{ get => _patches.Where(x => x.IsInstalled); }

	public IEnumerable<IHook> LoadedStaticHooks
	{ get => _staticHooks; }

	public IEnumerable<IHook> InstalledStaticHooks
	{ get => _staticHooks.Where(x => x.IsInstalled); }

	public IEnumerable<IHook> LoadedDynamicHooks
	{ get => _dynamicHooks; }

	public IEnumerable<IHook> InstalledDynamicHooks
	{ get => _dynamicHooks.Where(x => x.IsInstalled); }


	private bool HookIsSubscribedBy(string identifier, string subscriber)
		=> _subscribers?.Where(x => x.Identifier == identifier).Any(x => x.Subscriber == subscriber) ?? false;

	private bool HookHasSubscribers(string identifier)
		=> _subscribers?.Any(x => x.Identifier == identifier) ?? false;

	public int GetHookSubscriberCount(string identifier)
		=> _subscribers.Count(x => x.Identifier == identifier);

	private void AddSubscriber(string identifier, string subscriber)
		=> _subscribers.Add(item: new Subscription { Identifier = identifier, Subscriber = subscriber });

	private void RemoveSubscriber(string identifier, string subscriber)
		=> _subscribers.RemoveAll(x => x.Identifier == identifier && x.Subscriber == subscriber);


	public void Subscribe(string hookName, string requester)
	{
		try
		{
			HookEx single = GetHookById(hookName);

			if (single != null && !HookIsSubscribedBy(single.Identifier, requester))
			{
				Subscribe(single, requester);
				return;
			}

			IEnumerable<HookEx> hooks = GetHookByName(hookName);

			if (!hooks.Any())
			{
				Logger.Debug($"Failed to subscribe '{hookName}' by '{requester}', hook not found");
				return;
			};

			IEnumerable<HookEx> list = hooks
				.Where(hook => !HookIsSubscribedBy(hook.Identifier, requester));

			foreach (HookEx hook in list)
				Subscribe(hook, requester);

			list = default;
			hooks = default;
		}
		catch (Exception e)
		{
			Logger.Error($"Error while subscribing hook '{hookName}'", e);
			return;
		}
	}

	private void Subscribe(HookEx hook, string requester)
	{
		try
		{
			Logger.Debug($"Subscribe to '{hook}' by '{requester}'");

			foreach (HookEx dependency in GetHookDependencyTree(hook))
			{
				Logger.Debug($"Subscribe dependency '{dependency}' for '{hook}'", 1);
				AddSubscriber(dependency.Identifier, requester);
				Enqueue(dependency.Identifier);
			}

			AddSubscriber(hook.Identifier, requester);
			Enqueue(hook.Identifier);

			foreach (HookEx dependant in GetHookDependantTree(hook))
			{
				Logger.Debug($"Subscribe dependant '{dependant}' for '{hook}'", 1);
				AddSubscriber(dependant.Identifier, requester);
				Enqueue(dependant.Identifier);
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Error while subscribing hook '{hook}'", e);
			return;
		}
	}

	public void Unsubscribe(string hookName, string requester)
	{
		try
		{
			HookEx single = GetHookById(hookName);

			if (single != null && !HookIsSubscribedBy(single.Identifier, requester))
			{
				Unsubscribe(single, requester);
				return;
			}

			IEnumerable<HookEx> hooks = GetHookByName(hookName);

			if (!hooks.Any())
			{
				Logger.Debug($"Failure to subscribe to '{hookName}' by '{requester}', no hook found");
				return;
			};

			foreach (HookEx hook in hooks.Where(hook => HookIsSubscribedBy(hook.Identifier, requester)).Reverse())
				Unsubscribe(hook, requester);

			hooks = default;
		}
		catch (Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hookName}'", e);
			return;
		}
	}

	private void Unsubscribe(HookEx hook, string requester)
	{
		try
		{
			foreach (HookEx dependant in GetHookDependantTree(hook))
			{
				Logger.Debug($"Unsubscribe dependant '{dependant}' for '{hook}'", 1);
				RemoveSubscriber(dependant.Identifier, requester);
				Enqueue(dependant.Identifier);
			}

			RemoveSubscriber(hook.Identifier, requester);
			Enqueue(hook.Identifier);

			foreach (HookEx dependency in GetHookDependencyTree(hook))
			{
				Logger.Debug($"Unsubscribe dependency '{dependency}' for '{hook}'", 1);
				RemoveSubscriber(dependency.Identifier, requester);
				Enqueue(dependency.Identifier);
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Error while unsubscribing hook '{hook}'", e);
			return;
		}
	}

	public static string GetMethodMSILHash(MethodInfo method)
		=> SHA1(method?.GetMethodBody()?.GetILAsByteArray());

	public static string SHA1(byte[] raw)
	{
		if (raw == null || raw.Length == 0) return null;
		using SHA1Managed sha1 = new SHA1Managed();
		byte[] bytes = sha1.ComputeHash(raw);
		return string.Concat(bytes.Select(b => b.ToString("x2"))).ToLower();
	}

	private void CMDHookInfo(Command.Args arg)
	{
		if (!arg.Tokenize<ConsoleSystem.Arg>(out var args)) return;

		TextTable table = new();
		int count = 0, success = 0, warning = 0, failure = 0;

		string option1 = args.GetString(0, null);
		string option2 = args.GetString(1, null);

		table.AddColumns("#", "Name", "Hook", "Id", "Type", "Status", "Total", "Sub");

		switch (option1)
		{
			case "loaded":
				{
					IEnumerable<IHook> hooks;

					switch (option2)
					{
						case "--patch":
							hooks = Community.Runtime.HookManager.LoadedPatches.Where(x => !x.IsHidden);
							break;

						case "--static":
							hooks = Community.Runtime.HookManager.LoadedStaticHooks.Where(x => !x.IsHidden);
							break;

						case "--dynamic":
							hooks = Community.Runtime.HookManager.LoadedDynamicHooks.Where(x => !x.IsHidden);
							break;

						default:
#if DEBUG_VERBOSE
							hooks = Community.Runtime.HookManager.LoadedPatches;
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedStaticHooks);
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedDynamicHooks);
#else
							hooks = Community.Runtime.HookManager.LoadedPatches.Where(x => !x.IsHidden);
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedStaticHooks.Where(x => !x.IsHidden));
							hooks = hooks.Concat(Community.Runtime.HookManager.LoadedDynamicHooks.Where(x => !x.IsHidden));
#endif
							break;
					}

					switch (option2)
					{
						case "--usage":
							hooks = hooks.OrderByDescending(x => HookCaller.GetHookTotalTime(HookStringPool.GetOrAdd(x.HookName)));
							break;

						default:
							hooks = hooks.OrderBy(x => x.HookFullName);
							break;
					}

					foreach (var mod in hooks)
					{
						if (mod.Status == HookState.Failure) failure++;
						if (mod.Status == HookState.Success) success++;
						if (mod.Status == HookState.Warning) warning++;

						table.AddRow(
							$"{count++:n0}",
							mod.IsHidden
								? $"{mod.HookFullName} (*)"
								: mod.HookFullName,
							mod.HookName,
							mod.Identifier[^6..],
							mod.IsStaticHook
								? "Static"
								: mod.IsPatch ? "Patch" : "Dynamic",
							$"{mod.Status}",
							//$"{HookCaller.GetHookTime(mod.HookName)}ms",
							$"{HookCaller.GetHookTotalTime(HookStringPool.GetOrAdd(mod.HookName)):0}ms",
							(mod.IsStaticHook)
								? "N/A" :
								$"{Community.Runtime.HookManager.GetHookSubscriberCount(mod.Identifier),3}"
						);
					}

					arg.ReplyWith($"total:{count} success:{success} warning:{warning} failed:{failure}"
						+ Environment.NewLine + Environment.NewLine + table.ToString());
					break;
				}

			default: // list installed
				{
					IEnumerable<IHook> hooks;

					switch (option1)
					{
						case "--patch":
							hooks = Community.Runtime.HookManager.InstalledPatches.Where(x => !x.IsHidden);
							break;

						case "--static":
							hooks = Community.Runtime.HookManager.InstalledStaticHooks.Where(x => !x.IsHidden);
							break;

						case "--dynamic":
							hooks = Community.Runtime.HookManager.InstalledDynamicHooks.Where(x => !x.IsHidden);
							break;

						default:
#if DEBUG_VERBOSE
							hooks = Community.Runtime.HookManager.InstalledPatches;
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledStaticHooks);
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledDynamicHooks);
#else
							hooks = Community.Runtime.HookManager.InstalledPatches.Where(x => !x.IsHidden);
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledStaticHooks.Where(x => !x.IsHidden));
							hooks = hooks.Concat(Community.Runtime.HookManager.InstalledDynamicHooks.Where(x => !x.IsHidden));
#endif
							break;
					}

					if (option1 == "--usage" || option2 == "--usage")
					{
						hooks = hooks.OrderByDescending(x => HookCaller.GetHookTotalTime(HookStringPool.GetOrAdd(x.HookName)));
					}
					else
					{
						hooks = hooks.OrderBy(x => x.HookFullName);
					}

					foreach (var mod in hooks)
					{
						if (mod.Status == HookState.Failure) failure++;
						if (mod.Status == HookState.Success) success++;
						if (mod.Status == HookState.Warning) warning++;

						table.AddRow(
							$"{count++:n0}",
							mod.IsHidden
								? $"{mod.HookFullName} (*)"
								: mod.HookFullName,
							mod.HookName,
							mod.Identifier[^6..],
							mod.IsStaticHook
								? "Static"
								: mod.IsPatch ? "Patch" : "Dynamic",
							$"{mod.Status}",
							$"{HookCaller.GetHookTotalTime(HookStringPool.GetOrAdd(mod.HookName)):0}ms",
							(mod.IsStaticHook)
								? "N/A" :
								$"{Community.Runtime.HookManager.GetHookSubscriberCount(mod.Identifier),3}"
						);
					}

					arg.ReplyWith($"total:{count} success:{success} warning:{warning} failed:{failure}"
						+ Environment.NewLine + Environment.NewLine + table.ToString());
					break;
				}
		}
	}


	private bool _disposing;

	internal void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (HookEx item in _dynamicHooks) item.Dispose();
				_dynamicHooks = default;

				foreach (HookEx item in _installed) item.Dispose();
				_installed = default;

				foreach (HookEx item in _patches) item.Dispose();
				_patches = default;

				foreach (HookEx item in _staticHooks) item.Dispose();
				_staticHooks = default;

				_workQueue = default;
				_subscribers = default;
			}

			// unmanaged resources
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
