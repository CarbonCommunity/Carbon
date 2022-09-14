using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "AddSelfAuthorize" )]
    public class OnCupboardAuthorize
    {
        public static void Postfix ( BaseEntity.RPCMessage rpc , ref BuildingPrivlidge __instance )
        {
            HookExecutor.CallStaticHook ( "OnCupboardAuthorize" );
        }
    }
}