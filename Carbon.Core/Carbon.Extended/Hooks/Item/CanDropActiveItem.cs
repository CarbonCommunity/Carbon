using Carbon.Core;
using Harmony;
using Oxide.Core;

namespace Carbon.Extended
{
    [Hook ( "CanDropActiveItem", typeof ( bool ) ), Hook.Category ( Hook.Category.Enum.Item )]
    [Hook.Parameter ( "this", typeof ( BasePlayer ) )]
    [Hook.Info ( "Called when a player attempts to drop their active item." )]
    [Hook.Info ( "Returning true or false overrides default behavior." )]
    [HarmonyPatch ( typeof ( BasePlayer ), "ShouldDropActiveItem" )]
    public class BasePlayer_ShouldDropActiveItem
    {
        public static bool Prefix ( ref Item __instance, out bool __result )
        {
            var obj = Interface.CallHook ( "CanDropActiveItem", __instance );
            __result =  !( obj is bool ) || ( bool )obj;
            return false;
        }
    }
}