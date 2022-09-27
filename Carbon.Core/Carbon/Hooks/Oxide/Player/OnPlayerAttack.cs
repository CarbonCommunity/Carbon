///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using CompanionServer.Handlers;
using Harmony;
using Oxide.Core;
using ProtoBuf;

namespace Carbon.Extended
{
    [OxideHook ( "OnPlayerAttack", typeof ( object ) ), OxideHook.Category ( OxideHook.Category.Enum.Player )]
    [OxideHook.Parameter ( "player", typeof ( BasePlayer ) )]
    [OxideHook.Parameter ( "hitInfo", typeof ( HitInfo ) )]
    [OxideHook.Info ( "Useful for modifying an attack before it goes out." )]
    [OxideHook.Info ( "hitInfo.HitEntity should be the entity that this attack would hit." )]
    [OxideHook.Patch ( typeof ( BaseMelee ), "DoAttackShared" )]
    public class BaseMelee_DoAttackShared
    {
        public static bool Prefix ( HitInfo info, ref BaseMelee __instance )
        {
            return Interface.CallHook ( "OnPlayerAttack", __instance.GetOwnerPlayer (), info ) == null;
        }
    }
}