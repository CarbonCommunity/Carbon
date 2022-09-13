using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Workbench ), "ExperimentComplete" )]
    public class OnExperimentEnd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnExperimentEnd" );
        }
    }
}