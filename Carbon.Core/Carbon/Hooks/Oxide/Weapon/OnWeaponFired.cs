///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [OxideHook ( "OnWeaponFired" ), OxideHook.Category ( OxideHook.Category.Enum.Weapon )]
    [OxideHook.Parameter ( "projectile", typeof ( BaseProjectile ) )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "mod", typeof ( ItemModProjectile ) )]
    [OxideHook.Parameter ( "projectiles", typeof ( ProtoBuf.ProjectileShoot ) )]
    [OxideHook.Info ( "Called when the player fires a weapon." )]
    [OxideHook.Patch ( typeof ( BaseProjectile ), "CLProject" )]
    public class BaseProjectile_CLProject
    {
        public static void Prefix ( BaseEntity.RPCMessage msg, ref BaseProjectile __instance )
        {
            HookExecutor.CallStaticHook ( "OnWeaponFired", __instance, msg.player, __instance.PrimaryMagazineAmmo.GetComponent<ItemModProjectile> (), null );
        }
    }
}