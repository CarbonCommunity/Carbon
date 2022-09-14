using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "DoRepair" )]
    public class OnStructureRepair
    {
        public static void Postfix ( BasePlayer player )
        {
            HookExecutor.CallStaticHook ( "OnStructureRepair" );
        }
    }
}