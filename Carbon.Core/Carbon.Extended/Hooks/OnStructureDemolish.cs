using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoDemolish" )]
    public class OnStructureDemolish
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureDemolish" );
        }
    }
}