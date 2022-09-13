using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HelicopterTurret ), "SetTarget" )]
    public class OnHelicopterTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterTarget" );
        }
    }
}