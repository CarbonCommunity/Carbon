using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "AddBookmark" )]
    public class OnBookmarkAdd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkAdd" );
        }
    }
}