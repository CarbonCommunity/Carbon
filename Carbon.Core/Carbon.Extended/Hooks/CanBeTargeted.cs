using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GunTrap ), "CheckTrigger" )]
    public class CanBeTargeted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted" );
        }
    }
}