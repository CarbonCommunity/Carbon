using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "SplitItem" )]
    public class OnItemSplit
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemSplit" );
        }
    }
}