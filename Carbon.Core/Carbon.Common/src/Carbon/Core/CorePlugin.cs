using API.Events;
using Carbon.Client;
using ConVar;
using Application = UnityEngine.Application;
using CommandLine = Carbon.Components.CommandLine;
using Connection = Network.Connection;
using Timer = Oxide.Plugins.Timer;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;
#pragma warning disable IDE0051

public partial class CorePlugin : CarbonPlugin
{
	public override object InternalCallHook(uint hook, object[] args)
	{
		var result = (object)null;
		try
		{
			switch (hook)
			{
				// AddConditional aka 4026444072
				case 4026444072:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { AddConditional(arg0_0); }
						break;
					}
#if DEBUG
				// BeginProfile aka 3261118524
				case 3261118524:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { BeginProfile(arg0_0); }
						break;
					}
				// EndProfile aka 164269095
				case 164269095:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { EndProfile(arg0_0); }
						break;
					}
#endif
				// CarbonLoadConfig aka 815132798
				case 815132798:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { CarbonLoadConfig(arg0_0); }
						break;
					}
				// CarbonSaveConfig aka 2590157530
				case 2590157530:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { CarbonSaveConfig(arg0_0); }
						break;
					}
				// ClearMarkers aka 1438190503
				case 1438190503:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { ClearMarkers(arg0_0); }
						break;
					}
				// Conditionals aka 2768612739
				case 2768612739:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Conditionals(arg0_0); }
						break;
					}
				// Find aka 739121130
				case 739121130:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Find(arg0_0); }
						break;
					}
				// FindChat aka 3083923848
				case 3083923848:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { FindChat(arg0_0); }
						break;
					}
				// Grant aka 2312305252
				case 2312305252:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Grant(arg0_0); }
						break;
					}
				// Group aka 930025435
				case 930025435:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Group(arg0_0); }
						break;
					}
				// Help aka 2374729573
				case 2374729573:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Help(arg0_0); }
						break;
					}
				// ICanPickupEntity aka 3611612159
				case 3611612159:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is DoorCloser;
						var arg1_0 = narg1_0 ? (DoorCloser)(args[1] ?? (DoorCloser)default) : (DoorCloser)default;
						if (narg0_0 && narg1_0) { result = ICanPickupEntity(arg0_0, arg1_0); }
						break;
					}
				// ICraftDurationMultiplier aka 4130008882
				case 4130008882:
					{
						result = ICraftDurationMultiplier();
						break;
					}
				// IMixingSpeedMultiplier aka 2901256393
				case 2901256393:
					{
						var narg0_0 = args[0] is MixingTable;
						var arg0_0 = narg0_0 ? (MixingTable)(args[0] ?? (MixingTable)default) : (MixingTable)default;
						var narg1_0 = args[1] is float;
						var arg1_0 = narg1_0 ? (float)(args[1] ?? (float)default) : (float)default;
						if (narg0_0 && narg1_0) { result = IMixingSpeedMultiplier(arg0_0, arg1_0); }
						break;
					}
				// IOnBaseCombatEntityHurt aka 2404648208
				case 2404648208:
					{
						var narg0_0 = args[0] is BaseCombatEntity;
						var arg0_0 = narg0_0 ? (BaseCombatEntity)(args[0] ?? (BaseCombatEntity)default) : (BaseCombatEntity)default;
						var narg1_0 = args[1] is HitInfo;
						var arg1_0 = narg1_0 ? (HitInfo)(args[1] ?? (HitInfo)default) : (HitInfo)default;
						if (narg0_0 && narg1_0) { result = IOnBaseCombatEntityHurt(arg0_0, arg1_0); }
						break;
					}
				// IOnBasePlayerAttacked aka 447466736
				case 447466736:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is HitInfo;
						var arg1_0 = narg1_0 ? (HitInfo)(args[1] ?? (HitInfo)default) : (HitInfo)default;
						if (narg0_0 && narg1_0) { result = IOnBasePlayerAttacked(arg0_0, arg1_0); }
						break;
					}
				// IOnBasePlayerHurt aka 1334997006
				case 1334997006:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is HitInfo;
						var arg1_0 = narg1_0 ? (HitInfo)(args[1] ?? (HitInfo)default) : (HitInfo)default;
						if (narg0_0 && narg1_0) { result = IOnBasePlayerHurt(arg0_0, arg1_0); }
						break;
					}
				// IOnEntitySaved aka 284015616
				case 284015616:
					{
						var narg0_0 = args[0] is BaseNetworkable;
						var arg0_0 = narg0_0 ? (BaseNetworkable)(args[0] ?? (BaseNetworkable)default) : (BaseNetworkable)default;
						var narg1_0 = args[1] is BaseNetworkable.SaveInfo;
						var arg1_0 = narg1_0 ? (BaseNetworkable.SaveInfo)(args[1] ?? (BaseNetworkable.SaveInfo)default) : (BaseNetworkable.SaveInfo)default;
						if (narg0_0 && narg1_0) { IOnEntitySaved(arg0_0, arg1_0); }
						break;
					}
				// IOnExcavatorInit aka 1290758824
				case 1290758824:
					{
						var narg0_0 = args[0] is ExcavatorArm;
						var arg0_0 = narg0_0 ? (ExcavatorArm)(args[0] ?? (ExcavatorArm)default) : (ExcavatorArm)default;
						if (narg0_0) { result = IOnExcavatorInit(arg0_0); }
						break;
					}
				// IOnLoseCondition aka 1448274911
				case 1448274911:
					{
						var narg0_0 = args[0] is Item;
						var arg0_0 = narg0_0 ? (Item)(args[0] ?? (Item)default) : (Item)default;
						var narg1_0 = args[1] is float;
						var arg1_0 = narg1_0 ? (float)(args[1] ?? (float)default) : (float)default;
						if (narg0_0 && narg1_0) { result = IOnLoseCondition(arg0_0, arg1_0); }
						break;
					}
				// IOnNpcTarget aka 6843826
				case 6843826:
					{
						var narg0_0 = args[0] is BaseNpc;
						var arg0_0 = narg0_0 ? (BaseNpc)(args[0] ?? (BaseNpc)default) : (BaseNpc)default;
						var narg1_0 = args[1] is BaseEntity;
						var arg1_0 = narg1_0 ? (BaseEntity)(args[1] ?? (BaseEntity)default) : (BaseEntity)default;
						if (narg0_0 && narg1_0) { result = IOnNpcTarget(arg0_0, arg1_0); }
						break;
					}
				// IOnPlayerBanned aka 1154014332
				case 1154014332:
					{
						var narg0_0 = args[0] is Connection;
						var arg0_0 = narg0_0 ? (Connection)(args[0] ?? (Connection)default) : (Connection)default;
						var narg1_0 = args[1] is AuthResponse;
						var arg1_0 = narg1_0 ? (AuthResponse)(args[1] ?? (AuthResponse)default) : (AuthResponse)default;
						if (narg0_0 && narg1_0) { IOnPlayerBanned(arg0_0, arg1_0); }
						break;
					}
				// IOnPlayerChat aka 787516416
				case 787516416:
					{
						var narg0_0 = args[0] is ulong;
						var arg0_0 = narg0_0 ? (ulong)(args[0] ?? (ulong)default) : (ulong)default;
						var narg1_0 = args[1] is string;
						var arg1_0 = narg1_0 ? (string)(args[1] ?? (string)default) : (string)default;
						var narg2_0 = args[2] is string;
						var arg2_0 = narg2_0 ? (string)(args[2] ?? (string)default) : (string)default;
						var narg3_0 = args[3] is Chat.ChatChannel;
						var arg3_0 = narg3_0 ? (Chat.ChatChannel)(args[3] ?? (Chat.ChatChannel)default) : (Chat.ChatChannel)default;
						var narg4_0 = args[4] is BasePlayer;
						var arg4_0 = narg4_0 ? (BasePlayer)(args[4] ?? (BasePlayer)default) : (BasePlayer)default;
						if ((narg0_0 || args[0] == null) && (narg1_0 || args[1] == null) && (narg2_0 || args[2] == null) && narg3_0 && narg4_0) { result = IOnPlayerChat(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0); }
						break;
					}
				// IOnPlayerCommand aka 2581265021
				case 2581265021:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is string;
						var arg1_0 = narg1_0 ? (string)(args[1] ?? (string)default) : (string)default;
						if (narg0_0 && (narg1_0 || args[1] == null)) { result = IOnPlayerCommand(arg0_0, arg1_0); }
						break;
					}
				// IOnPlayerConnected aka 3691992858
				case 3691992858:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						if (narg0_0) { IOnPlayerConnected(arg0_0); }
						break;
					}
				// IOnServerCommand aka 2834650998
				case 2834650998:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { result = IOnServerCommand(arg0_0); }
						break;
					}
				// IOnServerInitialized aka 2521951123
				case 2521951123:
					{
						IOnServerInitialized();
						break;
					}
				// IOnServerShutdown aka 2994038319
				case 2994038319:
					{
						IOnServerShutdown();
						break;
					}
				// IOnUserApprove aka 2603852676
				case 2603852676:
					{
						var narg0_0 = args[0] is Connection;
						var arg0_0 = narg0_0 ? (Connection)(args[0] ?? (Connection)default) : (Connection)default;
						if (narg0_0) { result = IOnUserApprove(arg0_0); }
						break;
					}
				// IRecyclerThinkSpeed aka 880503512
				case 880503512:
					{
						result = IRecyclerThinkSpeed();
						break;
					}
				// IVendingBuyDuration aka 2959446098
				case 2959446098:
					{
						result = IVendingBuyDuration();
						break;
					}
				// LoadModuleConfig aka 646765377
				case 646765377:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { LoadModuleConfig(arg0_0); }
						break;
					}
				// LoadPlugin aka 1102288545
				case 1102288545:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { LoadPlugin(arg0_0); }
						break;
					}
				// Modules aka 2947791118
				case 2947791118:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Modules(arg0_0); }
						break;
					}
				// OnClientAuth aka 1280390023
				case 1280390023:
					{
						var narg0_0 = args[0] is Connection;
						var arg0_0 = narg0_0 ? (Connection)(args[0] ?? (Connection)default) : (Connection)default;
						if (narg0_0) { OnClientAuth(arg0_0); }
						break;
					}
				// OnEntityDeath aka 1215643893
				case 1215643893:
					{
						var narg0_0 = args[0] is BaseCombatEntity;
						var arg0_0 = narg0_0 ? (BaseCombatEntity)(args[0] ?? (BaseCombatEntity)default) : (BaseCombatEntity)default;
						var narg1_0 = args[1] is HitInfo;
						var arg1_0 = narg1_0 ? (HitInfo)(args[1] ?? (HitInfo)default) : (HitInfo)default;
						if (narg0_0 && narg1_0) { OnEntityDeath(arg0_0, arg1_0); }
						break;
					}
				// OnEntityKill aka 3950726597
				case 3950726597:
					{
						var narg0_0 = args[0] is BaseEntity;
						var arg0_0 = narg0_0 ? (BaseEntity)(args[0] ?? (BaseEntity)default) : (BaseEntity)default;
						if (narg0_0) { OnEntityKill(arg0_0); }
						break;
					}
				// OnEntitySpawned aka 3757549339
				case 3757549339:
					{
						var narg0_0 = args[0] is BaseEntity;
						var arg0_0 = narg0_0 ? (BaseEntity)(args[0] ?? (BaseEntity)default) : (BaseEntity)default;
						if (narg0_0) { OnEntitySpawned(arg0_0); }
						break;
					}
				// OnItemResearch aka 1330527334
				case 1330527334:
					{
						var narg0_0 = args[0] is ResearchTable;
						var arg0_0 = narg0_0 ? (ResearchTable)(args[0] ?? (ResearchTable)default) : (ResearchTable)default;
						var narg1_0 = args[1] is Item;
						var arg1_0 = narg1_0 ? (Item)(args[1] ?? (Item)default) : (Item)default;
						var narg2_0 = args[2] is BasePlayer;
						var arg2_0 = narg2_0 ? (BasePlayer)(args[2] ?? (BasePlayer)default) : (BasePlayer)default;
						if (narg0_0 && narg1_0 && narg2_0) { OnItemResearch(arg0_0, arg1_0, arg2_0); }
						break;
					}
				// OnPlayerDisconnected aka 2449451640
				case 2449451640:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is string;
						var arg1_0 = narg1_0 ? (string)(args[1] ?? (string)default) : (string)default;
						if (narg0_0 && (narg1_0 || args[1] == null)) { OnPlayerDisconnected(arg0_0, arg1_0); }
						break;
					}
				// OnPlayerKicked aka 713841014
				case 713841014:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						var narg1_0 = args[1] is string;
						var arg1_0 = narg1_0 ? (string)(args[1] ?? (string)default) : (string)default;
						if (narg0_0 && (narg1_0 || args[1] == null)) { OnPlayerKicked(arg0_0, arg1_0); }
						break;
					}
				// OnPlayerRespawn aka 1711034593
				case 1711034593:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						if (narg0_0) { result = OnPlayerRespawn(arg0_0); }
						break;
					}
				// OnPlayerRespawned aka 3404655920
				case 3404655920:
					{
						var narg0_0 = args[0] is BasePlayer;
						var arg0_0 = narg0_0 ? (BasePlayer)(args[0] ?? (BasePlayer)default) : (BasePlayer)default;
						if (narg0_0) { OnPlayerRespawned(arg0_0); }
						break;
					}
				// OnPlayerSetInfo aka 3042820326
				case 3042820326:
					{
						var narg0_0 = args[0] is Connection;
						var arg0_0 = narg0_0 ? (Connection)(args[0] ?? (Connection)default) : (Connection)default;
						var narg1_0 = args[1] is string;
						var arg1_0 = narg1_0 ? (string)(args[1] ?? (string)default) : (string)default;
						var narg2_0 = args[2] is string;
						var arg2_0 = narg2_0 ? (string)(args[2] ?? (string)default) : (string)default;
						if (narg0_0 && (narg1_0 || args[1] == null) && (narg2_0 || args[2] == null)) { OnPlayerSetInfo(arg0_0, arg1_0, arg2_0); }
						break;
					}
				// OnPluginLoaded aka 4143864509
				case 4143864509:
					{
						var narg0_0 = args[0] is Plugin;
						var arg0_0 = narg0_0 ? (Plugin)(args[0] ?? (Plugin)default) : (Plugin)default;
						if (narg0_0) { OnPluginLoaded(arg0_0); }
						break;
					}
				// OnPluginUnloaded aka 3843290135
				case 3843290135:
					{
						var narg0_0 = args[0] is Plugin;
						var arg0_0 = narg0_0 ? (Plugin)(args[0] ?? (Plugin)default) : (Plugin)default;
						if (narg0_0) { OnPluginUnloaded(arg0_0); }
						break;
					}
				// OnServerInitialized aka 1330569572
				case 1330569572:
					{
						OnServerInitialized();
						break;
					}
				// OnServerSave aka 2032593992
				case 2032593992:
					{
						OnServerSave();
						break;
					}
				// OnServerUserRemove aka 541418764
				case 541418764:
					{
						var narg0_0 = args[0] is ulong;
						var arg0_0 = narg0_0 ? (ulong)(args[0] ?? (ulong)default) : (ulong)default;
						if ((narg0_0 || args[0] == null)) { OnServerUserRemove(arg0_0); }
						break;
					}
				// OnServerUserSet aka 4207598011
				case 4207598011:
					{
						var narg0_0 = args[0] is ulong;
						var arg0_0 = narg0_0 ? (ulong)(args[0] ?? (ulong)default) : (ulong)default;
						var narg1_0 = args[1] is ServerUsers.UserGroup;
						var arg1_0 = narg1_0 ? (ServerUsers.UserGroup)(args[1] ?? (ServerUsers.UserGroup)default) : (ServerUsers.UserGroup)default;
						var narg2_0 = args[2] is string;
						var arg2_0 = narg2_0 ? (string)(args[2] ?? (string)default) : (string)default;
						var narg3_0 = args[3] is string;
						var arg3_0 = narg3_0 ? (string)(args[3] ?? (string)default) : (string)default;
						var narg4_0 = args[4] is long;
						var arg4_0 = narg4_0 ? (long)(args[4] ?? (long)default) : (long)default;
						if ((narg0_0 || args[0] == null) && narg1_0 && (narg2_0 || args[2] == null) && (narg3_0 || args[3] == null) && (narg4_0 || args[4] == null)) { OnServerUserSet(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0); }
						break;
					}
				// Plugins aka 2274417763
				case 2274417763:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Plugins(arg0_0); }
						break;
					}
				// PluginsFailed aka 2274439007
				case 2274439007:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { PluginsFailed(arg0_0); }
						break;
					}
				// PluginsUnloaded aka 1658890215
				case 1658890215:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { PluginsUnloaded(arg0_0); }
						break;
					}
				// PluginWarns aka 1175597617
				case 1175597617:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { PluginWarns(arg0_0); }
						break;
					}
				// Reboot aka 811300139
				case 811300139:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Reboot(arg0_0); }
						break;
					}
				// Reload aka 1720368164
				case 1720368164:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Reload(arg0_0); }
						break;
					}
				// ReloadConfig aka 74793642
				case 74793642:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { ReloadConfig(arg0_0); }
						break;
					}
				// RemoveConditional aka 3352136118
				case 3352136118:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { RemoveConditional(arg0_0); }
						break;
					}
				// Report aka 3116521
				case 3116521:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Report(arg0_0); }
						break;
					}
				// Revoke aka 3153338129
				case 3153338129:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Revoke(arg0_0); }
						break;
					}
				// SaveAllModules aka 467705630
				case 467705630:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { SaveAllModules(arg0_0); }
						break;
					}
				// SaveModuleConfig aka 1680578758
				case 1680578758:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { SaveModuleConfig(arg0_0); }
						break;
					}
				// SetModule aka 2689723201
				case 2689723201:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { SetModule(arg0_0); }
						break;
					}
				// Show aka 2970803623
				case 2970803623:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Show(arg0_0); }
						break;
					}
				// Shutdown aka 988816473
				case 988816473:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { Shutdown(arg0_0); }
						break;
					}
				// UnloadPlugin aka 1126445065
				case 1126445065:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { UnloadPlugin(arg0_0); }
						break;
					}
				// UserGroup aka 3375401029
				case 3375401029:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { UserGroup(arg0_0); }
						break;
					}
				// WipeUI aka 1544271710
				case 1544271710:
					{
						var narg0_0 = args[0] is ConsoleSystem.Arg;
						var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(args[0] ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
						if (narg0_0) { WipeUI(arg0_0); }
						break;
					}
			}
		}
		catch (System.Exception ex)
		{
			Carbon.Logger.Error($"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on plugin '{Name} v{Version}'", ex);
		}
		return result;
	}

	public static Dictionary<string, string> OrderedFiles { get; } = new Dictionary<string, string>();

	internal int _originalMaxPlayers = 0;

	public static void RefreshOrderedFiles()
	{
		OrderedFiles.Clear();

		var config = Community.Runtime.Config;
		var processor = Community.Runtime.ScriptProcessor;

		foreach (var file in OsEx.Folder.GetFilesWithExtension(Defines.GetScriptFolder(), "cs", config.ScriptWatcherOption))
		{
			if (processor.IsBlacklisted(file)) continue;

			var id = Path.GetFileNameWithoutExtension(file);
			if (!OrderedFiles.ContainsKey(id)) OrderedFiles.Add(id, file);
		}
	}

	public static string GetPluginPath(string shortName)
	{
		foreach (var file in OrderedFiles)
		{
			if (file.Key == shortName) return file.Value;
		}

		return null;
	}

	public override void IInit()
	{
		_defaultLogTrace = Application.GetStackTraceLogType(LogType.Log);
		_defaultWarningTrace = Application.GetStackTraceLogType(LogType.Warning);
		_defaultErrorTrace = Application.GetStackTraceLogType(LogType.Error);
		_defaultAssertTrace = Application.GetStackTraceLogType(LogType.Assert);
		_defaultExceptionTrace = Application.GetStackTraceLogType(LogType.Exception);

		ApplyStacktrace();

		Type = GetType();
		Hooks = new();

		foreach (var method in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
		{
			if (Community.Runtime.HookManager.IsHookLoaded(method.Name))
			{
				Community.Runtime.HookManager.Subscribe(method.Name, Name);

				var hash = HookStringPool.GetOrAdd(method.Name);
				if (!Hooks.Contains(hash)) Hooks.Add(hash);
			}
		}

		base.IInit();

		foreach (var player in BasePlayer.activePlayerList)
		{
			permission.RefreshUser(player);
		}

		timer.Every(5f, () =>
		{
			if (Community.Runtime == null || Logger.CoreLog == null || !Logger.CoreLog.HasInit || Logger.CoreLog._buffer.Count == 0 || Community.Runtime.Config.LogFileMode != 1) return;
			Logger.CoreLog.Flush();
		});

		cmd.AddConsoleCommand("help", this, nameof(Help), authLevel: 2);

		_originalMaxPlayers = ConVar.Server.maxplayers;
		ConVar.Server.maxplayers = 0;
	}

	private void OnServerInitialized()
	{
		Community.Runtime.ModuleProcessor.OnServerInit();
		CommandLine.ExecuteCommands("+carbon.onserverinit", "OnServerInitialized");

		var serverConfigPath = Path.Combine(ConVar.Server.GetServerFolder("cfg"), "server.cfg");
		var lines = OsEx.File.Exists(serverConfigPath) ? OsEx.File.ReadTextLines(serverConfigPath) : null;
		
		if (lines != null)
		{
			CommandLine.ExecuteCommands("+carbon.onserverinit", "cfg/server.cfg", lines);
			Array.Clear(lines, 0, lines.Length);
			lines = null;
		}

		var pluginCheck = (Timer)null;

		if (!Community.Runtime.ScriptProcessor.AllPendingScriptsComplete())
		{
			Logger.Warn($"There still are pending plugins loading... Temporarily disallowing players from joining (enabled queue).");
		}

		pluginCheck = timer.Every(1f, () =>
		{
			if (Community.Runtime.ScriptProcessor.AllPendingScriptsComplete() &&
				Community.Runtime.ScriptProcessor.AllNonRequiresScriptsComplete() &&
				Community.Runtime.ScriptProcessor.AllExtensionsComplete())
			{
				if (ConVar.Server.maxplayers != _originalMaxPlayers)
				{
					Logger.Warn($"All plugins have been loaded. Changing maximum players back to {_originalMaxPlayers}.");
					ConVar.Server.maxplayers = _originalMaxPlayers;
				}

				pluginCheck.Destroy();
				pluginCheck = null;
			}
		});

		CarbonAuto.Init();
		API.Abstracts.CarbonAuto.Singleton.Load();
	}
	private void OnServerSave()
	{
		Logger.Debug($"Saving Carbon state..", 1);
		Interface.Oxide.Permission.SaveData();
		Community.Runtime.ModuleProcessor.OnServerSave();

		Community.Runtime.Events
			.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);

		API.Abstracts.CarbonAuto.Singleton?.Save();
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		HookCaller.CallStaticHook(4253366379, player?.AsIPlayer(), reason);
	
		if (player.IsAdmin && !player.IsOnGround())
		{
			var newPosition = player.transform.position;

			if (UnityEngine.Physics.Raycast(newPosition, Vector3.down, out var hit, float.MaxValue, ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				newPosition.y = hit.point.y;

				if (Vector3.Distance(player.transform.position, newPosition) > 3.5f)
				{
					player.SetServerFall(false);
					player.Teleport(newPosition);
					player.estimatedVelocity = Vector3.zero;
					NextFrame(() =>
					{
						if (player != null)
						{
							player.SetServerFall(true);
						}
					});
					Logger.Warn($"Moved admin player {player.net.connection} on the object underneath so it doesn't die from fall damage.");
				}
			}
		}

		CarbonClient.Dispose(CarbonClient.Get(player));
	}
	private void OnPluginLoaded(Plugin plugin)
	{
		Community.Runtime.Events.Trigger(CarbonEvent.PluginLoaded, new CarbonEventArgs(plugin));
	}
	private void OnPluginUnloaded(Plugin plugin)
	{
		Community.Runtime.Events.Trigger(CarbonEvent.PluginUnloaded, new CarbonEventArgs(plugin));
	}
	private void OnEntitySpawned(BaseEntity entity)
	{
		Entities.AddMap(entity);
	}
	private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
	{
		Entities.RemoveMap(entity);
	}
	private void OnEntityKill(BaseEntity entity)
	{
		Entities.RemoveMap(entity);
	}

	internal static StackTraceLogType _defaultLogTrace;
	internal static StackTraceLogType _defaultWarningTrace;
	internal static StackTraceLogType _defaultErrorTrace;
	internal static StackTraceLogType _defaultAssertTrace;
	internal static StackTraceLogType _defaultExceptionTrace;

	public static void ApplyStacktrace()
	{
		if (Community.Runtime.Config.UnityStacktrace)
		{
			Application.SetStackTraceLogType(LogType.Log, _defaultLogTrace);
			Application.SetStackTraceLogType(LogType.Warning, _defaultWarningTrace);
			Application.SetStackTraceLogType(LogType.Error, _defaultErrorTrace);
			Application.SetStackTraceLogType(LogType.Assert, _defaultAssertTrace);
			Application.SetStackTraceLogType(LogType.Exception, _defaultExceptionTrace);
		}
		else
		{
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.None);
		}
	}
}
