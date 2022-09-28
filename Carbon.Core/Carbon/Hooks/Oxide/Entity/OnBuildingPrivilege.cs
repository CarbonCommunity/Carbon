///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "OnBuildingPrivilege", typeof ( BuildingPrivlidge ) ), OxideHook.Category ( Hook.Category.Enum.Entity )]
    [OxideHook.Parameter ( "entity", typeof ( BaseNetworkable ) )]
    [OxideHook.Info ( "Useful for overriding a building privilege on specific entities and etc.." )]
    [OxideHook.Info ( "Returning BuildingPrivlidge value overrides default behavior." )]
    [OxideHook.Patch ( typeof ( BaseEntity ), "GetBuildingPrivilege" )]
    public class BaseEntity_GetBuildingPrivilege
    {
        public static bool Prefix ( OBB obb, ref BaseNetworkable __instance, out BuildingPrivlidge __result )
        {
            var obj = HookExecutor.CallStaticHook ( "OnBuildingPrivilege", __instance, obb );

            if ( obj is BuildingPrivlidge )
            {
                __result = ( BuildingPrivlidge )obj;
                return false;
            }

            __result = null;
            return true;
        }
    }
}