using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "CreateProjectileEffectClientside" )]
    public class OnClientProjectileEffectCreate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnClientProjectileEffectCreate" );
        }
    }
}