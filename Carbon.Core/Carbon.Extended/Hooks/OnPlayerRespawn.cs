using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Respawn" )]
    public class OnPlayerRespawn
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawn" );
        }
    }
}