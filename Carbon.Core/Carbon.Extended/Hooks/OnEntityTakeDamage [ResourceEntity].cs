using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceEntity ), "OnAttacked" )]
    public class OnEntityTakeDamage [ResourceEntity]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityTakeDamage [ResourceEntity]" );
        }
    }
}