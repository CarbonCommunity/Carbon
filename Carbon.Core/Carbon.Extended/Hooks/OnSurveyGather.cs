using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SurveyCharge ), "Explode" )]
    public class OnSurveyGather
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSurveyGather" );
        }
    }
}