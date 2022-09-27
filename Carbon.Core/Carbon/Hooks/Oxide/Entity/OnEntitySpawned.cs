///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "OnEntitySpawned" ), OxideHook.Category ( Hook.Category.Enum.Entity )]
    [OxideHook.Parameter ( "entity", typeof ( BaseNetworkable ) )]
    [OxideHook.Info ( "Called after any networked entity has spawned (including trees)." )]
    [OxideHook.Patch ( typeof ( BaseNetworkable ), "Spawn" )]
    public class BaseNetworkable_Spawn_OnEntitySpawned
    {
        public static void Postfix ( ref BaseNetworkable __instance )
        {
            HookExecutor.CallStaticHook ( "OnEntitySpawned", __instance );
        }
    }
}