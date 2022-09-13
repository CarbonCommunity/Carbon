using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "StopControl" )]
    public class OnBookmarkControlEnd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkControlEnd" );
        }
    }
}