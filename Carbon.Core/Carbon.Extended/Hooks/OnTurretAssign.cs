using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "AssignToFriend" )]
    public class OnTurretAssign
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretAssign" );
        }
    }
}