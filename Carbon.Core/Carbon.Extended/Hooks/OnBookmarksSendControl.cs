using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "SendControlBookmarks" )]
    public class OnBookmarksSendControl
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarksSendControl" );
        }
    }
}