using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "BeginControllingBookmark" )]
    public class OnBookmarkControlStarted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkControlStarted" );
        }
    }
}