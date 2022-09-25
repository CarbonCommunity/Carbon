///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "OnPlayerInput" ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "input", typeof ( InputState ) )]
    [OxideHook.Info ( "Called when input is received from a connected client." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "OnReceiveTick" )]
    public class BasePlayer_OnReceiveTick
    {
        public static void Prefix ( PlayerTick msg, bool wasPlayerStalled, ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerInput", __instance, __instance.serverInput );
        }
    }
}