using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AntiHack ), "AddViolation" )]
    public class OnPlayerViolation
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerViolation" );
        }
    }
}