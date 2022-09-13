using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SleepingBag ), "SpawnPlayer" )]
    public class OnPlayerRespawn
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawn" );
        }
    }
}