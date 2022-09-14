using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConVar.Chat ), "Broadcast" )]
    public class OnServerMessage
    {
        public static bool Prefix ( System.String message, System.String username, System.String color, System.UInt64 userid )
        {
            return HookExecutor.CallStaticHook ( "OnServerMessage" ) == null;
        }
    }
}