using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "DoSimpleAI" )]
    public class OnBradleyApcThink
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBradleyApcThink" );
        }
    }
}