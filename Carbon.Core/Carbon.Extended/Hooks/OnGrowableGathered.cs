using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GrowableEntity ), "GiveFruit" )]
    public class OnGrowableGathered
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnGrowableGathered" );
        }
    }
}