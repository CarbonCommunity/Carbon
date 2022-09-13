using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity ), "UpdateNetworkGroup" )]
    public class NoLimboGroupForPlayers [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "NoLimboGroupForPlayers [patch]" );
        }
    }
}