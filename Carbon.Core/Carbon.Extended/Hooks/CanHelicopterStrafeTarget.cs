using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatrolHelicopterAI ), "ValidStrafeTarget" )]
    public class CanHelicopterStrafeTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanHelicopterStrafeTarget" );
        }
    }
}