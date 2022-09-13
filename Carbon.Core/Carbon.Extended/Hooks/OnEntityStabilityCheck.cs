using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( StabilityEntity ), "StabilityCheck" )]
    public class OnEntityStabilityCheck
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityStabilityCheck" );
        }
    }
}