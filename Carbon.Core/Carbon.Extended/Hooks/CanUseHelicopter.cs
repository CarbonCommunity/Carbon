using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "AttemptMount" )]
    public class CanUseHelicopter
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanUseHelicopter" );
        }
    }
}