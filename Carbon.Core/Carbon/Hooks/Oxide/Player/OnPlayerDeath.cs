///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnPlayerDeath", typeof(object) ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "info", typeof ( HitInfo ) )]
    [OxideHook.Info ( "Called when the player is about to die." )]
    [OxideHook.Info ( "HitInfo may be null sometimes." )]
    [OxideHook.Patch ( typeof ( BasePlayer ), "Die" )]
    public class BasePlayer_Die
    {
        public static bool Prefix ( HitInfo info, ref BasePlayer __instance )
        {
            if ( !__instance.IsDead () )
            {
                if ( __instance.Belt != null && __instance.ShouldDropActiveItem () )
                {
                    var vector = new Vector3 ( UnityEngine.Random.Range ( -2f, 2f ), 0.2f, UnityEngine.Random.Range ( -2f, 2f ) );
                    __instance.Belt.DropActive ( __instance.GetDropPosition (), __instance.GetInheritedDropVelocity () + vector.normalized * 3f );
                }
                if ( !__instance.WoundInsteadOfDying ( info ) )
                {
                    if ( Interface.CallHook ( "OnPlayerDeath", __instance, info ) != null )
                    {
                        return false;
                    }
                    SleepingBag.OnPlayerDeath ( __instance );
                    __instance.Die ( info );
                }
            }

            return false;
        }
    }
}