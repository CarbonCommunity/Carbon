using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RemoteControlEntity ), "CanControl" )]
    public class OnEntityControl
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityControl" );
        }
    }
}