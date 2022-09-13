using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCTalking ), "OnConversationAction" )]
    public class OnVendingShopOpen
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnVendingShopOpen" );
        }
    }
}