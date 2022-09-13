using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConsoleNetwork ), "BroadcastToAllClients" )]
    public class OnBroadcastCommand
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBroadcastCommand" );
        }
    }
}