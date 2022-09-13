using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class OnPlayerRevive
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerRevive" );
        }
    }
}