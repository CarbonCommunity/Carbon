using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "EligibleForWounding" )]
    public class CanBeWounded
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeWounded" );
        }
    }
}