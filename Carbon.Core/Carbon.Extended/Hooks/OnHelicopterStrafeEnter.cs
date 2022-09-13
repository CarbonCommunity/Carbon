using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatrolHelicopterAI ), "State_Strafe_Enter" )]
    public class OnHelicopterStrafeEnter
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterStrafeEnter" );
        }
    }
}