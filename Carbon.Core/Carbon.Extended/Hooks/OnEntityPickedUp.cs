using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "OnPickedUp" )]
    public class OnEntityPickedUp
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityPickedUp" );
        }
    }
}