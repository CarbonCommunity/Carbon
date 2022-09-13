using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoDemolish" )]
    public class OnStructureDemolish [immediate = false]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureDemolish [immediate = false]" );
        }
    }
}