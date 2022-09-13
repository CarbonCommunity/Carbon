using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseHelicopter ), "Hurt" )]
    public class OnHelicopterAttacked
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterAttacked" );
        }
    }
}