using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "DeleteBookmark" )]
    public class OnBookmarkDelete
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkDelete" );
        }
    }
}