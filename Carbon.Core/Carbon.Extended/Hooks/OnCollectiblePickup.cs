using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CollectibleEntity ), "DoPickup" )]
    public class OnCollectiblePickup
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCollectiblePickup" );
        }
    }
}