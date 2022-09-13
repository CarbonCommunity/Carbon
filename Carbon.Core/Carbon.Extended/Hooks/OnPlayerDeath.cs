using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Die" )]
    public class OnPlayerDeath
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerDeath" );
        }
    }
}