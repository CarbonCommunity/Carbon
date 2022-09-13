using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "ScrapForResearch" )]
    public class OnResearchCostDetermine [Item]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnResearchCostDetermine [Item]" );
        }
    }
}