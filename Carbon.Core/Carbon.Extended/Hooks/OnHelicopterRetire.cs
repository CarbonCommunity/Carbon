using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatrolHelicopterAI ), "Retire" )]
    public class OnHelicopterRetire
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterRetire" );
        }
    }
}