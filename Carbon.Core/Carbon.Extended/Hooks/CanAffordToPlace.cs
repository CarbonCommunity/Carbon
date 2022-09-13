using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Planner ), "CanAffordToPlace" )]
    public class CanAffordToPlace
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanAffordToPlace" );
        }
    }
}