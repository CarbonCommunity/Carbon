using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNpc ), "GetWantsToAttack" )]
    public class IOnNpcTarget [BaseNpc]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnNpcTarget [BaseNpc]" );
        }
    }
}