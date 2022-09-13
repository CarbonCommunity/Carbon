using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoDemolish" )]
    public class OnStructureDemolish [false, patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureDemolish [false, patch]" );
        }
    }
}