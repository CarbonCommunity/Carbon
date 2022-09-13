using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "SpawnPlayer" )]
    public class OnPlayerRespawn [SleepingBag]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawn [SleepingBag]" );
        }
    }
}