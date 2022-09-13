using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMelee ), "CLProject" )]
    public class OnMeleeThrown
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMeleeThrown" );
        }
    }
}