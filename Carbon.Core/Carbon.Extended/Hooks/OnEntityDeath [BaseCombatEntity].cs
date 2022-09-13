using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "Die" )]
    public class OnEntityDeath [BaseCombatEntity]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath [BaseCombatEntity]" );
        }
    }
}