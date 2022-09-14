using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "ResearchAttemptFinished" )]
    public class OnItemResearched
    {
        public static void Postfix ( , ref ResearchTable __instance )
        {
            HookExecutor.CallStaticHook ( "OnItemResearched" );
        }
    }
}