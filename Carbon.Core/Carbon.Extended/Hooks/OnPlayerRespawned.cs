using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RespawnAt" )]
    public class OnPlayerRespawned
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawned" );
        }
    }
}