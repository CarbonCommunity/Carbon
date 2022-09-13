using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCTalking ), "Server_BeginTalking" )]
    public class OnNpcConversationStart
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcConversationStart" );
        }
    }
}