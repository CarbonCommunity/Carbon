using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "ScrapForResearch" )]
    public class OnResearchCostDetermine
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnResearchCostDetermine" );
        }
    }
}