using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "CanDemolish" )]
    public class CanDemolish
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanDemolish" );
        }
    }
}