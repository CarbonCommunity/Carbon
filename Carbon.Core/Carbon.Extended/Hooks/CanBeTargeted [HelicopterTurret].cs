using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HelicopterTurret ), "InFiringArc" )]
    public class CanBeTargeted [HelicopterTurret]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted [HelicopterTurret]" );
        }
    }
}