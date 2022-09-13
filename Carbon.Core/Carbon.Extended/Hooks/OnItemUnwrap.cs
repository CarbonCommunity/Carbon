using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemModUnwrap ), "ServerCommand" )]
    public class OnItemUnwrap
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemUnwrap" );
        }
    }
}