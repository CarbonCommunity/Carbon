using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Workbench ), "RPC_TechTreeUnlock" )]
    public class OnTechTreeNodeUnlock
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTechTreeNodeUnlock" );
        }
    }
}