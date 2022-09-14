using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ChatMessage" )]
    public class OnMessagePlayer
    {
        public static void Postfix ( System.String msg , ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnMessagePlayer" );
        }
    }
}