using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GrowableEntity ), "PickFruit" )]
    public class OnGrowableGather
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnGrowableGather" );
        }
    }
}