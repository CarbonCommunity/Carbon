using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Facepunch.Rcon.Listener ), "Start" )]
    public class OnRconConnection
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRconConnection" );
        }
    }
}