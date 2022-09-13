using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConsoleSystem ), "Internal" )]
    public class IOnServerCommand
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "IOnServerCommand" );
        }
    }
}