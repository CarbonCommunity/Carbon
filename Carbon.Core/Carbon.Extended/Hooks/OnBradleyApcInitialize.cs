using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "Initialize" )]
    public class OnBradleyApcInitialize
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBradleyApcInitialize" );
        }
    }
}