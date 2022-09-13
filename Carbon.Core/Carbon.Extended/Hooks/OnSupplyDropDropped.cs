using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CargoPlane ), "Update" )]
    public class OnSupplyDropDropped
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSupplyDropDropped" );
        }
    }
}