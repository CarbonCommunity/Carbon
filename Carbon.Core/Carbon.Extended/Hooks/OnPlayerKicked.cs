using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Kick" )]
    public class OnPlayerKicked
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerKicked" );
        }
    }
}