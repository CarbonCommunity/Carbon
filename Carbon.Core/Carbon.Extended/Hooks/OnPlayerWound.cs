using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "BecomeWounded" )]
    public class OnPlayerWound
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerWound" );
        }
    }
}