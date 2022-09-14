using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "RemoveSelfAuthorize" )]
    public class OnCupboardDeauthorize
    {
        public static void Postfix ( BaseEntity.RPCMessage rpc , ref BuildingPrivlidge __instance )
        {
            HookExecutor.CallStaticHook ( "OnCupboardDeauthorize" );
        }
    }
}