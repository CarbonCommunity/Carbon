using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "CreateWorldProjectile" )]
    public class OnWorldProjectileCreate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWorldProjectileCreate" );
        }
    }
}