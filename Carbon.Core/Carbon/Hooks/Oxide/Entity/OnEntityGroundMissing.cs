///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;

namespace Carbon.Extended
{
    [OxideHook ( "OnEntityGroundMissing", typeof ( bool ) ), OxideHook.Category ( Hook.Category.Enum.Entity )]
    [OxideHook.Parameter ( "entity", typeof ( BaseEntity ) )]
    [OxideHook.Info ( "Called when an entity is going to be destroyed because the buildingblock it is on was removed." )]
    [OxideHook.Patch ( typeof ( DestroyOnGroundMissing ), "OnGroundMissing" )]
    public class DestroyOnGroundMissing_OnGroundMissing
    {
        public static bool Prefix ( ref DestroyOnGroundMissing __instance )
        {
            var entity = __instance.gameObject.ToBaseEntity ();
            if ( entity == null ) return false;

            return Interface.CallHook ( "OnEntityGroundMissing", entity ) == null;
        }
    }
}