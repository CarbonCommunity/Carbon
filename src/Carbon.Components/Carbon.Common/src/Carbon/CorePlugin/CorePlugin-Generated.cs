using Newtonsoft.Json;
using HarmonyLib;
using API.Commands;
using Facepunch;
using Oxide.Game.Rust.Cui;
using Carbon.Test;
using System.Text;
using API.Hooks;
using Timer = Oxide.Plugins.Timer;
using Carbon.Base.Interfaces;
using Application = UnityEngine.Application;
using System.Xml.Linq;
using API.Events;
using Carbon.Profiler;
using ConVar;
using Command = API.Commands.Command;
using Connection = Network.Connection;
using Rust.Ai.Gen2;
using CommandLine = Carbon.Components.CommandLine;

namespace Carbon.Core;
public partial class CorePlugin
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        var narg1 = length > 1 ? args[1] : null;
        var narg2 = length > 2 ? args[2] : null;
        var narg3 = length > 3 ? args[3] : null;
        var narg4 = length > 4 ? args[4] : null;
        try
        {
            switch (hook)
            {
                // AddConditional aka 3617829410
                case 3617829410:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        AddConditional(arg0_0);
                        return null;
                    }

                    break;
                }

                // Aliases aka 1461696666
                case 1461696666:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Aliases(arg0_0);
                        return null;
                    }

                    break;
                }

                // AssignAlias aka 1310794640
                case 1310794640:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        AssignAlias(arg0_0);
                        return null;
                    }

                    break;
                }

                // BuildCall aka 1569187096
                case 1569187096:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        BuildCall(arg0_0);
                        return null;
                    }

                    break;
                }

                // CanUnlockTechTreeNode aka 307092880
                case 307092880:
                {
#if !MINIMAL
                    return CanUnlockTechTreeNode();
#endif
                    break;
                }

                // CarbonLoadConfig aka 2522567266
                case 2522567266:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CarbonLoadConfig(arg0_0);
                        return null;
                    }

                    break;
                }

                // CarbonSaveConfig aka 8097725
                case 8097725:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CarbonSaveConfig(arg0_0);
                        return null;
                    }

                    break;
                }

                // ChangeVersion aka 3026698837
                case 3026698837:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ChangeVersion(arg0_0);
                        return null;
                    }

                    break;
                }

                // ClearMarkers aka 2486811342
                case 2486811342:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ClearMarkers(arg0_0);
                        return null;
                    }

                    break;
                }

                // Commit aka 212981081
                case 212981081:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Commit(arg0_0);
                        return null;
                    }

                    break;
                }

                // Conditionals aka 121761328
                case 121761328:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Conditionals(arg0_0);
                        return null;
                    }

                    break;
                }

                // CreatePlugin aka 1813333766
                case 1813333766:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CreatePlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // DebugAllHooks aka 3825563089
                case 3825563089:
                {
#if DEBUG
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        DebugAllHooks(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // DebugHook aka 382008794
                case 382008794:
                {
#if DEBUG
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        DebugHook(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // DebugHookImpl aka 2700798693
                case 2700798693:
                {
#if DEBUG
                    var narg0_0 = narg0 is string or null;
                    var arg0_0 = narg0_0 ? (string)(narg0 ?? (string)default) : (string)default;
                    var narg1_0 = narg1 is float or null;
                    var arg1_0 = narg1_0 ? (float)(narg1 ?? (float)default) : (float)default;
                    if (narg0_0 && narg1_0)
                    {
                        DebugHookImpl(arg0_0, arg1_0, out var arg2_0);
                        args[2] = arg2_0;
                        return null;
                    }

#endif
                    break;
                }

                // Delete aka 2563024626
                case 2563024626:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Delete(arg0_0);
                        return null;
                    }

                    break;
                }

                // DeleteExt aka 1566035725
                case 1566035725:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        DeleteExt(arg0_0);
                        return null;
                    }

                    break;
                }

                // DevDumpSnapshot aka 2179227208
                case 2179227208:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        DevDumpSnapshot(arg0_0);
                        return null;
                    }

                    break;
                }

                // EditConfig aka 2215751651
                case 2215751651:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        EditConfig(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // Extensions aka 1012871006
                case 1012871006:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Extensions(arg0_0);
                        return null;
                    }

                    break;
                }

                // Find aka 2557278796
                case 2557278796:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Find(arg0_0);
                        return null;
                    }

                    break;
                }

                // FindChat aka 2822243214
                case 2822243214:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        FindChat(arg0_0);
                        return null;
                    }

                    break;
                }

                // FireHook aka 2693900894
                case 2693900894:
                {
#if DEBUG
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        FireHook(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // GenerateInternal aka 70584118
                case 70584118:
                {
#if DEBUG
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        GenerateInternal(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // GetVault aka 3993433097
                case 3993433097:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        GetVault(arg0_0);
                        return null;
                    }

                    break;
                }

                // GetWebControlPanelClients aka 4128227484
                case 4128227484:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        GetWebControlPanelClients(arg0_0);
                        return null;
                    }

                    break;
                }

                // GoCommunity aka 2362460257
                case 2362460257:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        GoCommunity(arg0_0);
                        return null;
                    }

                    break;
                }

                // Grant aka 3167076070
                case 3167076070:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Grant(arg0_0);
                        return null;
                    }

                    break;
                }

                // Group aka 879858435
                case 879858435:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Group(arg0_0);
                        return null;
                    }

                    break;
                }

                // HarmonyMods aka 126019937
                case 126019937:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        HarmonyMods(arg0_0);
                        return null;
                    }

                    break;
                }

                // Help aka 1224025706
                case 1224025706:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Help(arg0_0);
                        return null;
                    }

                    break;
                }

                // HookInfo aka 2465598932
                case 2465598932:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        HookInfo(arg0_0);
                        return null;
                    }

                    break;
                }

                // HooksCall aka 1110553926
                case 1110553926:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        HooksCall(arg0_0);
                        return null;
                    }

                    break;
                }

                // ICraftDurationMultiplier aka 2966092386
                case 2966092386:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ItemBlueprint or null;
                    var arg0_0 = narg0_0 ? (ItemBlueprint)(narg0 ?? (ItemBlueprint)default) : (ItemBlueprint)default;
                    var narg1_0 = narg1 is float or null;
                    var arg1_0 = narg1_0 ? (float)(narg1 ?? (float)default) : (float)default;
                    var narg2_0 = narg2 is bool or null;
                    var arg2_0 = narg2_0 ? (bool)(narg2 ?? (bool)default) : (bool)default;
                    if (narg0_0 && narg1_0 && narg2_0)
                    {
                        return ICraftDurationMultiplier(arg0_0, arg1_0, arg2_0);
                    }

#endif
                    break;
                }

                // IMixingSpeedMultiplier aka 1211592203
                case 1211592203:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is MixingTable or null;
                    var arg0_0 = narg0_0 ? (MixingTable)(narg0 ?? (MixingTable)default) : (MixingTable)default;
                    var narg1_0 = narg1 is float or null;
                    var arg1_0 = narg1_0 ? (float)(narg1 ?? (float)default) : (float)default;
                    if (narg0_0 && narg1_0)
                    {
                        return IMixingSpeedMultiplier(arg0_0, arg1_0);
                    }

#endif
                    break;
                }

                // InstallPlugin aka 2370971930
                case 2370971930:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        InstallPlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // IOnExcavatorInit aka 1112160822
                case 1112160822:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ExcavatorArm or null;
                    var arg0_0 = narg0_0 ? (ExcavatorArm)(narg0 ?? (ExcavatorArm)default) : (ExcavatorArm)default;
                    if (narg0_0)
                    {
                        IOnExcavatorInit(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // IOnServerInitialized aka 4155259925
                case 4155259925:
                {
                    var narg0_0 = narg0 is bool or null;
                    var arg0_0 = narg0_0 ? (bool)(narg0 ?? (bool)default) : (bool)default;
                    if (narg0_0)
                    {
                        return IOnServerInitialized(arg0_0);
                    }

                    break;
                }

                // IOvenSmeltSpeedMultiplier aka 3923985155
                case 3923985155:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseOven or null;
                    var arg0_0 = narg0_0 ? (BaseOven)(narg0 ?? (BaseOven)default) : (BaseOven)default;
                    if (narg0_0)
                    {
                        return IOvenSmeltSpeedMultiplier(arg0_0);
                    }

#endif
                    break;
                }

                // IRecyclerThinkSpeed aka 3134346010
                case 3134346010:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Recycler or null;
                    var arg0_0 = narg0_0 ? (Recycler)(narg0 ?? (Recycler)default) : (Recycler)default;
                    if (narg0_0)
                    {
                        return IRecyclerThinkSpeed(arg0_0);
                    }

#endif
                    break;
                }

                // IResearchDuration aka 1870500310
                case 1870500310:
                {
#if !MINIMAL
                    IResearchDuration();
                    return null;
#endif
                    break;
                }

                // IVendingBuyDuration aka 4192766051
                case 4192766051:
                {
#if !MINIMAL
                    return IVendingBuyDuration();
#endif
                    break;
                }

                // LoadDefaultMessages aka 313256762
                case 313256762:
                {
                    LoadDefaultMessages();
                    return null;
                    break;
                }

                // LoadModule aka 1175002629
                case 1175002629:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        LoadModule(arg0_0);
                        return null;
                    }

                    break;
                }

                // LoadPlugin aka 2699051938
                case 2699051938:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        LoadPlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // LoadWebControlPanelConfig aka 3768797615
                case 3768797615:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        LoadWebControlPanelConfig(arg0_0);
                        return null;
                    }

                    break;
                }

                // MigrateToProto aka 2009737156
                case 2009737156:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        MigrateToProto(arg0_0);
                        return null;
                    }

                    break;
                }

                // MigrateToSql aka 2539568543
                case 2539568543:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        MigrateToSql(arg0_0);
                        return null;
                    }

                    break;
                }

                // ModdedRustConVars aka 4209386718
                case 4209386718:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ModdedRustConVars(arg0_0);
                        return null;
                    }

                    break;
                }

                // ModuleInfo aka 3694325137
                case 3694325137:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ModuleInfo(arg0_0);
                        return null;
                    }

                    break;
                }

                // Modules aka 346822591
                case 346822591:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Modules(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnClientAuth aka 2263673102
                case 2263673102:
                {
                    var narg0_0 = narg0 is Connection or null;
                    var arg0_0 = narg0_0 ? (Connection)(narg0 ?? (Connection)default) : (Connection)default;
                    if (narg0_0)
                    {
                        OnClientAuth(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerChat aka 2032160890
                case 2032160890:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    var narg2_0 = narg2 is ConVar.Chat.ChatChannel or null;
                    var arg2_0 = narg2_0 ? (ConVar.Chat.ChatChannel)(narg2 ?? (ConVar.Chat.ChatChannel)default) : (ConVar.Chat.ChatChannel)default;
                    if (narg0_0 && narg1_0 && narg2_0)
                    {
                        OnPlayerChat(arg0_0, arg1_0, arg2_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerDisconnected aka 72085565
                case 72085565:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnPlayerDisconnected(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerKicked aka 1321158727
                case 1321158727:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnPlayerKicked(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerRespawn aka 1546340674
                case 1546340674:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        return OnPlayerRespawn(arg0_0);
                    }

                    break;
                }

                // OnPlayerRespawned aka 458523914
                case 458523914:
                {
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        OnPlayerRespawned(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnPlayerSetInfo aka 2283023029
                case 2283023029:
                {
                    var narg0_0 = narg0 is Connection or null;
                    var arg0_0 = narg0_0 ? (Connection)(narg0 ?? (Connection)default) : (Connection)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    var narg2_0 = narg2 is string or null;
                    var arg2_0 = narg2_0 ? (string)(narg2 ?? (string)default) : (string)default;
                    if (narg0_0 && narg1_0 && narg2_0)
                    {
                        OnPlayerSetInfo(arg0_0, arg1_0, arg2_0);
                        return null;
                    }

                    break;
                }

                // OnPluginLoaded aka 3051933177
                case 3051933177:
                {
                    var narg0_0 = narg0 is RustPlugin or null;
                    var arg0_0 = narg0_0 ? (RustPlugin)(narg0 ?? (RustPlugin)default) : (RustPlugin)default;
                    if (narg0_0)
                    {
                        OnPluginLoaded(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnPluginUnloaded aka 1250294368
                case 1250294368:
                {
                    var narg0_0 = narg0 is RustPlugin or null;
                    var arg0_0 = narg0_0 ? (RustPlugin)(narg0 ?? (RustPlugin)default) : (RustPlugin)default;
                    if (narg0_0)
                    {
                        OnPluginUnloaded(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnSaveLoad aka 106238856
                case 106238856:
                {
                    OnSaveLoad();
                    return null;
                    break;
                }

                // OnServerInitialized aka 352240293
                case 352240293:
                {
                    OnServerInitialized();
                    return null;
                    break;
                }

                // OnServerSave aka 2396958305
                case 2396958305:
                {
                    OnServerSave();
                    return null;
                    break;
                }

                // OnServerUserRemove aka 2043356880
                case 2043356880:
                {
                    var narg0_0 = narg0 is ulong or null;
                    var arg0_0 = narg0_0 ? (ulong)(narg0 ?? (ulong)default) : (ulong)default;
                    if (narg0_0)
                    {
                        OnServerUserRemove(arg0_0);
                        return null;
                    }

                    break;
                }

                // OnServerUserSet aka 931424179
                case 931424179:
                {
                    var narg0_0 = narg0 is ulong or null;
                    var arg0_0 = narg0_0 ? (ulong)(narg0 ?? (ulong)default) : (ulong)default;
                    var narg1_0 = narg1 is ServerUsers.UserGroup or null;
                    var arg1_0 = narg1_0 ? (ServerUsers.UserGroup)(narg1 ?? (ServerUsers.UserGroup)default) : (ServerUsers.UserGroup)default;
                    var narg2_0 = narg2 is string or null;
                    var arg2_0 = narg2_0 ? (string)(narg2 ?? (string)default) : (string)default;
                    var narg3_0 = narg3 is string or null;
                    var arg3_0 = narg3_0 ? (string)(narg3 ?? (string)default) : (string)default;
                    var narg4_0 = narg4 is long or null;
                    var arg4_0 = narg4_0 ? (long)(narg4 ?? (long)default) : (long)default;
                    if (narg0_0 && narg1_0 && narg2_0 && narg3_0 && narg4_0)
                    {
                        OnServerUserSet(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0);
                        return null;
                    }

                    break;
                }

                // OnTeamMemberPromote aka 1658239813
                case 1658239813:
                {
                    var narg0_0 = narg0 is RelationshipManager.PlayerTeam or null;
                    var arg0_0 = narg0_0 ? (RelationshipManager.PlayerTeam)(narg0 ?? (RelationshipManager.PlayerTeam)default) : (RelationshipManager.PlayerTeam)default;
                    var narg1_0 = narg1 is ulong or null;
                    var arg1_0 = narg1_0 ? (ulong)(narg1 ?? (ulong)default) : (ulong)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnTeamMemberPromote(arg0_0, arg1_0);
                        return null;
                    }

                    break;
                }

                // PluginCmds aka 1853185048
                case 1853185048:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        PluginCmds(arg0_0);
                        return null;
                    }

                    break;
                }

                // PluginInfo aka 2730263207
                case 2730263207:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        PluginInfo(arg0_0);
                        return null;
                    }

                    break;
                }

                // Plugins aka 1778989243
                case 1778989243:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Plugins(arg0_0);
                        return null;
                    }

                    break;
                }

                // Profile aka 1503455692
                case 1503455692:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Profile(arg0_0);
                        return null;
                    }

                    break;
                }

                // ProfileAbort aka 869177234
                case 869177234:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfileAbort(arg0_0);
                        return null;
                    }

                    break;
                }

                // ProfilerPrint aka 2235018487
                case 2235018487:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerPrint(arg0_0);
                        return null;
                    }

                    break;
                }

                // ProfilerRemovePlugin aka 1282370872
                case 1282370872:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerRemovePlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // ProfilerTracked aka 1603732279
                case 1603732279:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerTracked(arg0_0);
                        return null;
                    }

                    break;
                }

                // ProfilerTrackPlugin aka 1400659095
                case 1400659095:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerTrackPlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // Protocol aka 4118252168
                case 4118252168:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Protocol(arg0_0);
                        return null;
                    }

                    break;
                }

                // Reload aka 1669471309
                case 1669471309:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Reload(arg0_0);
                        return null;
                    }

                    break;
                }

                // ReloadConfig aka 407700441
                case 407700441:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ReloadConfig(arg0_0);
                        return null;
                    }

                    break;
                }

                // ReloadModule aka 3607291287
                case 3607291287:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ReloadModule(arg0_0);
                        return null;
                    }

                    break;
                }

                // RemoveConditional aka 165571558
                case 165571558:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        RemoveConditional(arg0_0);
                        return null;
                    }

                    break;
                }

                // ResetHooks aka 3053121748
                case 3053121748:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ResetHooks(arg0_0);
                        return null;
                    }

                    break;
                }

                // Revoke aka 1983852833
                case 1983852833:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Revoke(arg0_0);
                        return null;
                    }

                    break;
                }

                // SaveModule aka 3408782460
                case 3408782460:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        SaveModule(arg0_0);
                        return null;
                    }

                    break;
                }

                // SaveWebControlPanelConfig aka 523784464
                case 523784464:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        SaveWebControlPanelConfig(arg0_0);
                        return null;
                    }

                    break;
                }

                // SayAs aka 2303807553
                case 2303807553:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        SayAs(arg0_0);
                        return null;
                    }

                    break;
                }

                // SetModule aka 4206019811
                case 4206019811:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        SetModule(arg0_0);
                        return null;
                    }

                    break;
                }

                // Show aka 3296300873
                case 3296300873:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Show(arg0_0);
                        return null;
                    }

                    break;
                }

                // Shutdown aka 414928410
                case 414928410:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Shutdown(arg0_0);
                        return null;
                    }

                    break;
                }

                // Skin aka 1867912083
                case 1867912083:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Skin(arg0_0);
                        return null;
                    }

                    break;
                }

                // test_beds aka 1250811332
                case 1250811332:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        test_beds(arg0_0);
                        return null;
                    }

                    break;
                }

                // test_clear aka 4236641972
                case 4236641972:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        test_clear(arg0_0);
                        return null;
                    }

                    break;
                }

                // test_collect aka 473670306
                case 473670306:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        test_collect(arg0_0);
                        return null;
                    }

                    break;
                }

                // test_plugin aka 683585953
                case 683585953:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        test_plugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // test_run aka 2426236348
                case 2426236348:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        test_run(arg0_0);
                        return null;
                    }

                    break;
                }

                // TryToggleWebControlPanelServer aka 2977018099
                case 2977018099:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        TryToggleWebControlPanelServer(arg0_0);
                        return null;
                    }

                    break;
                }

                // UnassignAlias aka 275530773
                case 275530773:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        UnassignAlias(arg0_0);
                        return null;
                    }

                    break;
                }

                // UninstallPlugin aka 3547710285
                case 3547710285:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        UninstallPlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // UnloadPlugin aka 1342457948
                case 1342457948:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        UnloadPlugin(arg0_0);
                        return null;
                    }

                    break;
                }

                // UserGroup aka 2108743044
                case 2108743044:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        UserGroup(arg0_0);
                        return null;
                    }

                    break;
                }

                // VaultAdd aka 297788813
                case 297788813:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        VaultAdd(arg0_0);
                        return null;
                    }

                    break;
                }

                // VaultRemove aka 1579191406
                case 1579191406:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        VaultRemove(arg0_0);
                        return null;
                    }

                    break;
                }

                // VersionCall aka 3912190147
                case 3912190147:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        VersionCall(arg0_0);
                        return null;
                    }

                    break;
                }

                // WhyModded aka 340806103
                case 340806103:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        WhyModded(arg0_0);
                        return null;
                    }

                    break;
                }

                // WipeUI aka 1847800369
                case 1847800369:
                {
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        WipeUI(arg0_0);
                        return null;
                    }

                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Carbon.Logger.Error($"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on plugin '{base.Name} v{base.Version}' [{hook}]", ex);
            OnException(hook);
        }

        return (object)null;
    }
}