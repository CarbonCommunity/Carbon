using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Planner ), "PayForPlacement" )]
    public class OnPayForPlacement
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPayForPlacement" );
        }
    }
}