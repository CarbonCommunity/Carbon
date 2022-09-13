using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MedicalTool ), "GiveEffectsTo" )]
    public class OnHealingItemUse
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnHealingItemUse" );
        }
    }
}