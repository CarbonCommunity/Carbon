using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SupplyDrop ), "OnCollisionEnter" )]
    public class OnSupplyDropLanded
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSupplyDropLanded" );
        }
    }
}