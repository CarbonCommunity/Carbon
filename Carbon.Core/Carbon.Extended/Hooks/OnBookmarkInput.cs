using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ComputerStation ), "PlayerServerInput" )]
    public class OnBookmarkInput
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBookmarkInput" );
        }
    }
}