using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "IsHostile" )]
    public class CanEntityBeHostile
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanEntityBeHostile" );
        }
    }
}