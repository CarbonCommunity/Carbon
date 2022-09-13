using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConsoleNetwork ), "OnClientCommand" )]
    public class OnClientCommand
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnClientCommand" );
        }
    }
}