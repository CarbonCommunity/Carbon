using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoUpgradeToGrade" )]
    public class OnStructureUpgrade
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnStructureUpgrade" );
        }
    }
}