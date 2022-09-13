using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "ResearchAttemptFinished" )]
    public class OnItemResearched
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemResearched" );
        }
    }
}