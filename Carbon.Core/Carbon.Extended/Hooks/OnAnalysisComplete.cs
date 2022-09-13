using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SurveyCrater ), "AnalysisComplete" )]
    public class OnAnalysisComplete
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnAnalysisComplete" );
        }
    }
}