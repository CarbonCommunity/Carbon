using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "Drop" )]
    public class OnItemDropped
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemDropped" );
        }
    }
}