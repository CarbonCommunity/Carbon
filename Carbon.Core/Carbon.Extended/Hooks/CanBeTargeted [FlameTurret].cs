using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FlameTurret ), "CheckTrigger" )]
    public class CanBeTargeted [FlameTurret]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted [FlameTurret]" );
        }
    }
}