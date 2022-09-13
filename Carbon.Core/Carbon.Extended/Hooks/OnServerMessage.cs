using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConVar.Chat ), "Broadcast" )]
    public class OnServerMessage
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnServerMessage" );
        }
    }
}