using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCTalking ), "Server_EndTalking" )]
    public class OnNpcConversationEnded
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcConversationEnded" );
        }
    }
}