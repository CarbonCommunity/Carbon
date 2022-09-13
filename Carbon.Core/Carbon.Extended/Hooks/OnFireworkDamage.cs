using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFirework ), "OnAttacked" )]
    public class OnFireworkDamage
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFireworkDamage" );
        }
    }
}