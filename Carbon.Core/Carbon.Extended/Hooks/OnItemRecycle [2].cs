using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Recycler ), "RecycleThink" )]
    public class OnItemRecycle [2]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemRecycle [2]" );
        }
    }
}