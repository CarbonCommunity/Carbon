using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MixingTable ), "SVSwitch" )]
    public class OnMixingTableToggle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnMixingTableToggle" );
        }
    }
}