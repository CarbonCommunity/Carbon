using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "SendUpdate" )]
    public class OnLootNetworkUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLootNetworkUpdate" );
        }
    }
}