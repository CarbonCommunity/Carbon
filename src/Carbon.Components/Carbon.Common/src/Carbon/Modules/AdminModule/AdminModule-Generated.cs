using ConVar;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using UnityEngine.UI;
using static Carbon.Components.CUI;
using static ConsoleSystem;
using Color = UnityEngine.Color;
using StringEx = Carbon.Extensions.StringEx;
using ExtensionMethods = Oxide.Core.ExtensionMethods;
using API.Hooks;
using Facepunch;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using API.Commands;
using API.Abstracts;
using Network;
using TimeEx = Carbon.Extensions.TimeEx;
using ProtoBuf;
using System.IO.Compression;
using System.Text;
using Exception = System.Exception;
using Timer = Oxide.Plugins.Timer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Carbon.Modules;
public partial class AdminModule
{
    public override object InternalCallHook(uint hook, object[] args)
    {
        var length = args?.Length;
        var narg0 = length > 0 ? args[0] : null;
        var narg1 = length > 1 ? args[1] : null;
        var narg2 = length > 2 ? args[2] : null;
        var narg3 = length > 3 ? args[3] : null;
        var narg4 = length > 4 ? args[4] : null;
        var narg5 = length > 5 ? args[5] : null;
        var narg6 = length > 6 ? args[6] : null;
        var narg7 = length > 7 ? args[7] : null;
        var narg8 = length > 8 ? args[8] : null;
        try
        {
            switch (hook)
            {
                // CallAction aka 3567215104
                case 3567215104:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CallAction(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // CanAcceptItem aka 1360889797
                case 1360889797:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ItemContainer or null;
                    var arg0_0 = narg0_0 ? (ItemContainer)(narg0 ?? (ItemContainer)default) : (ItemContainer)default;
                    var narg1_0 = narg1 is Item or null;
                    var arg1_0 = narg1_0 ? (Item)(narg1 ?? (Item)default) : (Item)default;
                    var narg2_0 = narg2 is int or null;
                    var arg2_0 = narg2_0 ? (int)(narg2 ?? (int)default) : (int)default;
                    if (narg0_0 && narg1_0 && narg2_0)
                    {
                        return CanAcceptItem(arg0_0, arg1_0, arg2_0);
                    }

#endif
                    break;
                }

                // CanAccess aka 3983791359
                case 3983791359:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        return CanAccess(arg0_0);
                    }

#endif
                    break;
                }

                // ChangeColumnPage aka 2673788810
                case 2673788810:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ChangeColumnPage(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ChangePage aka 2461250588
                case 2461250588:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ChangePage(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ChangeTab aka 2281510334
                case 2281510334:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ChangeTab(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // CloseUI aka 4151318565
                case 4151318565:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        CloseUI(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // Dialog_Action aka 1294167637
                case 1294167637:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Dialog_Action(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // DownloadPlugin aka 821139628
                case 821139628:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        DownloadPlugin(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // IModBackpack aka 3944193281
                case 3944193281:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseMountable or null;
                    var arg0_0 = narg0_0 ? (BaseMountable)(narg0 ?? (BaseMountable)default) : (BaseMountable)default;
                    var narg1_0 = narg1 is BasePlayer or null;
                    var arg1_0 = narg1_0 ? (BasePlayer)(narg1 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0 && narg1_0)
                    {
                        return IModBackpack(arg0_0, arg1_0);
                    }

#endif
                    break;
                }

                // ItemClear aka 2737989634
                case 2737989634:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ItemClear(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ItemCreate aka 3688899430
                case 3688899430:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ItemCreate(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ItemSetting aka 4063786885
                case 4063786885:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ItemSetting(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // IValidDismountPosition aka 513549662
                case 513549662:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseMountable or null;
                    var arg0_0 = narg0_0 ? (BaseMountable)(narg0 ?? (BaseMountable)default) : (BaseMountable)default;
                    var narg1_0 = narg1 is BasePlayer or null;
                    var arg1_0 = narg1_0 ? (BasePlayer)(narg1 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0 && narg1_0)
                    {
                        return IValidDismountPosition(arg0_0, arg1_0);
                    }

#endif
                    break;
                }

                // Maximize aka 347761043
                case 347761043:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        Maximize(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnEntityDismounted aka 2026747374
                case 2026747374:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseMountable or null;
                    var arg0_0 = narg0_0 ? (BaseMountable)(narg0 ?? (BaseMountable)default) : (BaseMountable)default;
                    var narg1_0 = narg1 is BasePlayer or null;
                    var arg1_0 = narg1_0 ? (BasePlayer)(narg1 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0 && narg1_0)
                    {
                        OnEntityDismounted(arg0_0, arg1_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnEntityDistanceCheck aka 1582967250
                case 1582967250:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseEntity or null;
                    var arg0_0 = narg0_0 ? (BaseEntity)(narg0 ?? (BaseEntity)default) : (BaseEntity)default;
                    var narg1_0 = narg1 is BasePlayer or null;
                    var arg1_0 = narg1_0 ? (BasePlayer)(narg1 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg2_0 = narg2 is uint or null;
                    var arg2_0 = narg2_0 ? (uint)(narg2 ?? (uint)default) : (uint)default;
                    var narg3_0 = narg3 is string or null;
                    var arg3_0 = narg3_0 ? (string)(narg3 ?? (string)default) : (string)default;
                    var narg4_0 = narg4 is float or null;
                    var arg4_0 = narg4_0 ? (float)(narg4 ?? (float)default) : (float)default;
                    if (narg0_0 && narg1_0 && narg2_0 && narg3_0 && narg4_0)
                    {
                        return OnEntityDistanceCheck(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0);
                    }

#endif
                    break;
                }

                // OnEntityVisibilityCheck aka 3141188509
                case 3141188509:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BaseEntity or null;
                    var arg0_0 = narg0_0 ? (BaseEntity)(narg0 ?? (BaseEntity)default) : (BaseEntity)default;
                    var narg1_0 = narg1 is BasePlayer or null;
                    var arg1_0 = narg1_0 ? (BasePlayer)(narg1 ?? (BasePlayer)default) : (BasePlayer)default;
                    var narg2_0 = narg2 is uint or null;
                    var arg2_0 = narg2_0 ? (uint)(narg2 ?? (uint)default) : (uint)default;
                    var narg3_0 = narg3 is string or null;
                    var arg3_0 = narg3_0 ? (string)(narg3 ?? (string)default) : (string)default;
                    var narg4_0 = narg4 is float or null;
                    var arg4_0 = narg4_0 ? (float)(narg4 ?? (float)default) : (float)default;
                    if (narg0_0 && narg1_0 && narg2_0 && narg3_0 && narg4_0)
                    {
                        return OnEntityVisibilityCheck(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0);
                    }

#endif
                    break;
                }

                // OnLog aka 3088593565
                case 3088593565:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is string or null;
                    var arg0_0 = narg0_0 ? (string)(narg0 ?? (string)default) : (string)default;
                    var narg1_0 = narg1 is string or null;
                    var arg1_0 = narg1_0 ? (string)(narg1 ?? (string)default) : (string)default;
                    var narg2_0 = narg2 is LogType or null;
                    var arg2_0 = narg2_0 ? (LogType)(narg2 ?? (LogType)default) : (LogType)default;
                    if (narg0_0 && narg1_0 && narg2_0)
                    {
                        OnLog(arg0_0, arg1_0, arg2_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnPlayerDisconnected aka 72085565
                case 72085565:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is BasePlayer or null;
                    var arg0_0 = narg0_0 ? (BasePlayer)(narg0 ?? (BasePlayer)default) : (BasePlayer)default;
                    if (narg0_0)
                    {
                        OnPlayerDisconnected(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnPlayerLootEnd aka 78733418
                case 78733418:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is PlayerLoot or null;
                    var arg0_0 = narg0_0 ? (PlayerLoot)(narg0 ?? (PlayerLoot)default) : (PlayerLoot)default;
                    if (narg0_0)
                    {
                        OnPlayerLootEnd(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnPluginLoaded aka 3051933177
                case 3051933177:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is RustPlugin or null;
                    var arg0_0 = narg0_0 ? (RustPlugin)(narg0 ?? (RustPlugin)default) : (RustPlugin)default;
                    if (narg0_0)
                    {
                        OnPluginLoaded(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // OnPluginUnloaded aka 1250294368
                case 1250294368:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is RustPlugin or null;
                    var arg0_0 = narg0_0 ? (RustPlugin)(narg0 ?? (RustPlugin)default) : (RustPlugin)default;
                    if (narg0_0)
                    {
                        OnPluginUnloaded(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserChange aka 1575289668
                case 1575289668:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserChange(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserChangeSelected aka 1844112064
                case 1844112064:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserChangeSelected(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserChangeSetting aka 2086187233
                case 2086187233:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserChangeSetting(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserCloseLogin aka 3307168968
                case 3307168968:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserCloseLogin(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserDeselectPlugin aka 3216105140
                case 3216105140:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserDeselectPlugin(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserInteract aka 2137231192
                case 2137231192:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserInteract(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserLogin aka 3865171010
                case 3865171010:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserLogin(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserPage aka 1494079807
                case 1494079807:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserPage(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserRefreshVendor aka 1286856038
                case 1286856038:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserRefreshVendor(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserSearch aka 3027434422
                case 3027434422:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserSearch(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // PluginBrowserSelectPlugin aka 1756664287
                case 1756664287:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        PluginBrowserSelectPlugin(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerClear aka 1200368304
                case 1200368304:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerClear(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerExport aka 4013736532
                case 4013736532:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerExport(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerImport aka 1104199099
                case 1104199099:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerImport(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerPreviewClose aka 939287569
                case 939287569:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerPreviewClose(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerSelect aka 1496207780
                case 1496207780:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerSelect(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerSelectCall aka 3990053074
                case 3990053074:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerSelectCall(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ProfilerToggle aka 3314827294
                case 3314827294:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ProfilerToggle(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ShowConfig aka 2369215218
                case 2369215218:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ShowConfig(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // ShowProfiler aka 1081434670
                case 1081434670:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        ShowProfiler(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // TabButton aka 1462991777
                case 1462991777:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is CUI or null;
                    var arg0_0 = narg0_0 ? (CUI)(narg0 ?? (CUI)default) : (CUI)default;
                    var narg1_0 = narg1 is CuiElementContainer or null;
                    var arg1_0 = narg1_0 ? (CuiElementContainer)(narg1 ?? (CuiElementContainer)default) : (CuiElementContainer)default;
                    var narg2_0 = narg2 is string or null;
                    var arg2_0 = narg2_0 ? (string)(narg2 ?? (string)default) : (string)default;
                    var narg3_0 = narg3 is string or null;
                    var arg3_0 = narg3_0 ? (string)(narg3 ?? (string)default) : (string)default;
                    var narg4_0 = narg4 is string or null;
                    var arg4_0 = narg4_0 ? (string)(narg4 ?? (string)default) : (string)default;
                    var narg5_0 = narg5 is float or null;
                    var arg5_0 = narg5_0 ? (float)(narg5 ?? (float)default) : (float)default;
                    var narg6_0 = narg6 is float or null;
                    var arg6_0 = narg6_0 ? (float)(narg6 ?? (float)default) : (float)default;
                    if (narg0_0 && narg1_0 && narg2_0 && narg3_0 && narg4_0 && narg5_0 && narg6_0)
                    {
                        TabButton(arg0_0, arg1_0, arg2_0, arg3_0, arg4_0, arg5_0, arg6_0, narg7 is bool arg7_0 ? arg7_0 : (bool)default, narg8 is bool arg8_0 ? arg8_0 : (bool)default);
                        return null;
                    }

#endif
                    break;
                }

                // TimelineClear aka 1709151936
                case 1709151936:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        TimelineClear(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // TimelineMode aka 1042898331
                case 1042898331:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        TimelineMode(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // TimelineToggle aka 1088631640
                case 1088631640:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is ConsoleSystem.Arg or null;
                    var arg0_0 = narg0_0 ? (ConsoleSystem.Arg)(narg0 ?? (ConsoleSystem.Arg)default) : (ConsoleSystem.Arg)default;
                    if (narg0_0)
                    {
                        TimelineToggle(arg0_0);
                        return null;
                    }

#endif
                    break;
                }

                // UpdateVendor aka 1322676003
                case 1322676003:
                {
#if !MINIMAL
                    var narg0_0 = narg0 is Arg or null;
                    var arg0_0 = narg0_0 ? (Arg)(narg0 ?? (Arg)default) : (Arg)default;
                    if (narg0_0)
                    {
                        UpdateVendor(arg0_0);
                        return null;
                    }

#endif
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Carbon.Logger.Error($"Failed to call internal hook '{Carbon.Pooling.HookStringPool.GetOrAdd(hook)}' on module '{this.Name} v{this.Version}' [{hook}]", ex);
            OnException(hook);
        }

        return (object)null;
    }
}