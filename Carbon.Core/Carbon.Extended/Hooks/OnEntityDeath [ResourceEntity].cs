using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceEntity ), "OnKilled" )]
    public class OnEntityDeath [ResourceEntity]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath [ResourceEntity]" );
        }
    }
}