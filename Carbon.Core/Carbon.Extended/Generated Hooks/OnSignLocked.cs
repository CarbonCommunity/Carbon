using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "LockSign" )]
    public class OnSignLocked
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref Signage __instance )
        {
            HookExecutor.CallStaticHook ( "OnSignLocked" );
        }
    }
}