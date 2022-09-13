using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "Die" )]
    public class OnEntityDeath
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDeath" );
        }
    }
}