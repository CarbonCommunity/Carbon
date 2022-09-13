using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GrowableEntity ), "ChangeState" )]
    public class OnGrowableStateChange
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnGrowableStateChange" );
        }
    }
}