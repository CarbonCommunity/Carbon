using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "CanBeLooted" )]
    public class CanLootPlayer
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootPlayer" );
        }
    }
}