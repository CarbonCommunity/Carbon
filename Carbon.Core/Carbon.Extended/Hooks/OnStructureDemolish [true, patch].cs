using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoImmediateDemolish" )]
    public class OnStructureDemolish [true, patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureDemolish [true, patch]" );
        }
    }
}