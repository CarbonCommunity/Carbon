using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "DoResearch" )]
    public class CanResearchItem
    {
        public static bool Prefix ( BaseEntity.RPCMessage msg , ref ResearchTable __instance )
        {
            return HookExecutor.CallStaticHook ( "CanResearchItem", __instance, l1, l0 ) == null;
        }
    }
}