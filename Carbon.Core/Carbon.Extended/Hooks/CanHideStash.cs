using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( StashContainer ), "RPC_HideStash" )]
    public class CanHideStash
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanHideStash" );
        }
    }
}