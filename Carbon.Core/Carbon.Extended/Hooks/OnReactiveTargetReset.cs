using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ReactiveTarget ), "ResetTarget" )]
    public class OnReactiveTargetReset
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnReactiveTargetReset" );
        }
    }
}