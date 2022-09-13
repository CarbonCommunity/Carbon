using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Planner ), "CanAffordToPlace" )]
    public class CanAffordToPlace
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAffordToPlace" );
        }
    }
}