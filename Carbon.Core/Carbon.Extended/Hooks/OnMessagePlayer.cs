using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ChatMessage" )]
    public class OnMessagePlayer
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMessagePlayer" );
        }
    }
}