using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "BeginControllingBookmark" )]
    public class OnBookmarkControl
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkControl" );
        }
    }
}