using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Recycler ), "SVSwitch" )]
    public class OnRecyclerToggle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRecyclerToggle" );
        }
    }
}