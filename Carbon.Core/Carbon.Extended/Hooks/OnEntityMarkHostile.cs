using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "MarkHostileFor" )]
    public class OnEntityMarkHostile
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityMarkHostile" );
        }
    }
}