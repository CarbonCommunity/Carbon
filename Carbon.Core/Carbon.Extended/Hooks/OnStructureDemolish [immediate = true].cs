using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoImmediateDemolish" )]
    public class OnStructureDemolish [immediate = true]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureDemolish [immediate = true]" );
        }
    }
}