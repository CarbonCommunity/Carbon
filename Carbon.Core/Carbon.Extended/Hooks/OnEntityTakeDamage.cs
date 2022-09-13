using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceEntity ), "OnAttacked" )]
    public class OnEntityTakeDamage
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityTakeDamage" );
        }
    }
}