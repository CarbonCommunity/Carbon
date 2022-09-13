using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "EndSleeping" )]
    public class OnPlayerSleepEnd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSleepEnd" );
        }
    }
}