using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "Clear" )]
    public class OnPlayerLootEnd
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerLootEnd" );
        }
    }
}