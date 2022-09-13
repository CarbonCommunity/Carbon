using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Door ), "RPC_OpenDoor" )]
    public class OnDoorOpened
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDoorOpened" );
        }
    }
}