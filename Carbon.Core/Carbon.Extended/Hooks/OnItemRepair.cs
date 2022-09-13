using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RepairBench ), "RepairAnItem" )]
    public class OnItemRepair
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemRepair" );
        }
    }
}