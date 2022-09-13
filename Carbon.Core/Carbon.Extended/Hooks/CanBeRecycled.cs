using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Recycler ), "CanBeRecycled" )]
    public class CanBeRecycled
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeRecycled" );
        }
    }
}