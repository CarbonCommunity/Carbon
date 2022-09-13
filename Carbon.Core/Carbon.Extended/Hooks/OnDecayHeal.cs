using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DecayEntity ), "DecayTick" )]
    public class OnDecayHeal
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDecayHeal" );
        }
    }
}