using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseLauncher ), "SV_Launch" )]
    public class OnRocketLaunched
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRocketLaunched" );
        }
    }
}