using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class OnPlayerRevive [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRevive [patch]" );
        }
    }
}