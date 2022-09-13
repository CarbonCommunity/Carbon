using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCTalking ), "Server_ResponsePressed" )]
    public class OnNpcConversationRespond
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcConversationRespond" );
        }
    }
}