using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RecoverFromWounded" )]
    public class OnPlayerRecover
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRecover" );
        }
    }
}