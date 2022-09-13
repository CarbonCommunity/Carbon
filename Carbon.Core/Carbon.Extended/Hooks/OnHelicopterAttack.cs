using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "OnAttacked" )]
    public class OnHelicopterAttack
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterAttack" );
        }
    }
}