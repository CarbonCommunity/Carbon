using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "UpdateSign" )]
    public class OnSignUpdated
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref Signage __instance )
        {
            HookExecutor.CallStaticHook ( "OnSignUpdated" );
        }
    }
}