using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResearchTable ), "DoResearch" )]
    public class OnItemResearch
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemResearch" );
        }
    }
}