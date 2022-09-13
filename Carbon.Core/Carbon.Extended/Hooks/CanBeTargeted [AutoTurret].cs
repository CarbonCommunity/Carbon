using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "ObjectVisible" )]
    public class CanBeTargeted [AutoTurret]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted [AutoTurret]" );
        }
    }
}