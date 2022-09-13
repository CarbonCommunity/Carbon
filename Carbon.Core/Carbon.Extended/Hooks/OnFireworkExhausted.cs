using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseFirework ), "OnExhausted" )]
    public class OnFireworkExhausted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFireworkExhausted" );
        }
    }
}