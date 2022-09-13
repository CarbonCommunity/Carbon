using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DoorCloser ), "RPC_Take" )]
    public class ICanPickupEntity
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "ICanPickupEntity" );
        }
    }
}