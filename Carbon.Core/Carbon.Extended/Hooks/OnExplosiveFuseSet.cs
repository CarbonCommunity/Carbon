using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TimedExplosive ), "SetFuse" )]
    public class OnExplosiveFuseSet
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnExplosiveFuseSet" );
        }
    }
}