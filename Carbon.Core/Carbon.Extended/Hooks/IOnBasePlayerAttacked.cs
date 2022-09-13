using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnAttacked" )]
    public class IOnBasePlayerAttacked
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnBasePlayerAttacked" );
        }
    }
}