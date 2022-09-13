using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "DoRepair" )]
    public class OnStructureRepair
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureRepair" );
        }
    }
}