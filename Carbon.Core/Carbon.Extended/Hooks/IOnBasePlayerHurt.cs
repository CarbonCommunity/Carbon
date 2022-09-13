using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "Hurt" )]
    public class IOnBasePlayerHurt
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnBasePlayerHurt" );
        }
    }
}