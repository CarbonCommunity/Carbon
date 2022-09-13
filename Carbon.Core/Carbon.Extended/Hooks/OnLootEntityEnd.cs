using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemBasedFlowRestrictor ), "PlayerStoppedLooting" )]
    public class OnLootEntityEnd
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLootEntityEnd" );
        }
    }
}