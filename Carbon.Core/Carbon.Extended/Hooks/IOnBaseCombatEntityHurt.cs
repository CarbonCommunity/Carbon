using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseCombatEntity ), "Hurt" )]
    public class IOnBaseCombatEntityHurt
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "IOnBaseCombatEntityHurt" );
        }
    }
}