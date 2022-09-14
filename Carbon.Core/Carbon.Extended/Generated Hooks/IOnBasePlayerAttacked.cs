using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnAttacked" )]
    public class IOnBasePlayerAttacked
    {
        public static bool Prefix ( HitInfo info )
        {
            return HookExecutor.CallStaticHook ( "IOnBasePlayerAttacked" ) == null;
        }
    }
}