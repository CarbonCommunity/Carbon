using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WorldItem ), "Pickup" )]
    public class OnItemPickup
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemPickup" );
        }
    }
}