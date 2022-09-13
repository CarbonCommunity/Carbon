using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Network.Server ), "OnDisconnected" )]
    public class OnClientDisconnected
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnClientDisconnected" );
        }
    }
}