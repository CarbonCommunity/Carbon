using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Facepunch.Rcon.Listener ), "Start" )]
    public class IOnRconCommand
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnRconCommand" );
        }
    }
}