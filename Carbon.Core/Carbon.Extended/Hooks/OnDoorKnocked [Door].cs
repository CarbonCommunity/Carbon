using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Door ), "RPC_KnockDoor" )]
    public class OnDoorKnocked [Door]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnDoorKnocked [Door]" );
        }
    }
}