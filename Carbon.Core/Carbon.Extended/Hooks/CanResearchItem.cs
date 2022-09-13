using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "DoResearch" )]
    public class CanResearchItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanResearchItem" );
        }
    }
}