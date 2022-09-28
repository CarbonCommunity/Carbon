///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "OnQuarryToggled", typeof ( object ) ), OxideHook.Category ( Hook.Category.Enum.Resources )]
    [OxideHook.Parameter ( "this", typeof ( EngineSwitch ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Info ( "Called when a quarry has just been toggled." )]
    [OxideHook.Patch ( typeof ( EngineSwitch ), "StartEngine" )]
    public class EngineSwitch_StartEngine
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref EngineSwitch __instance )
        {
            var miningQuarry = __instance.GetParentEntity () as global::MiningQuarry;

            if ( miningQuarry != null )
            {
                miningQuarry.EngineSwitch ( true );
                return HookExecutor.CallStaticHook ( "OnQuarryToggled", miningQuarry, msg.player ) == null;
            }

            return false;
        }
    }

    [Hook ( "OnQuarryToggled", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Resources )]
    [Hook.Parameter ( "this", typeof ( EngineSwitch ) )]
    [Hook.Parameter ( "player", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when a quarry has just been toggled." )]
    [Hook.Patch ( typeof ( EngineSwitch ), "StopEngine" )]
    public class EngineSwitch_StopEngine
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg, ref EngineSwitch __instance )
        {
            var miningQuarry = __instance.GetParentEntity () as global::MiningQuarry;

            if ( miningQuarry != null )
            {
                miningQuarry.EngineSwitch ( false );
                return HookExecutor.CallStaticHook ( "OnQuarryToggled", miningQuarry, msg.player ) == null;
            }

            return false;
        }
    }
}